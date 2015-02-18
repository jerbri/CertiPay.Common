namespace CertiPay.Database.Maintenance
{
    using CertiPay.Common.Logging;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using DeviceType = Microsoft.SqlServer.Management.Smo.DeviceType;

    /// <summary>
    /// Performs a full backup of the configured database, verifying it once complete
    /// </summary>
    public class FullBackup
    {
        private static readonly ILog Log = LogManager.GetLogger<FullBackup>();

        public class Configuration
        {
            public String ConnectionString { get; set; }

            public String OutputFileName { get; set; }

            public Boolean Compression { get; set; }

            public Boolean IncrementalBackup { get; set; }

            public Boolean VerifyBackup { get; set; }

            public Configuration()
            {
                this.Compression = true;
                this.VerifyBackup = true;
            }
        }

        public void Execute(Configuration configuration)
        {
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection) { StatementTimeout = 0 });
                Database database = new Database(server, connection.Database);

                Log.Info("Starting database backup job");

                // Loads the table/index information
                database.Refresh();

                // Check database integrity ~ DBCC CheckDB
                WeeklyMaintenance.CheckDatabase(database);

                String destination = Path.Combine(server.BackupDirectory, configuration.OutputFileName);

                // Perform backup
                BackupDatabase(server, database, configuration, destination);

                Log.Info("Finished backup.");

                if (configuration.VerifyBackup)
                {
                    // Verify backup was completed and is usable
                    if (!VerifyBackup(server, destination))
                    {
                        throw new Exception("Error verifying latest sql backup!");
                    }
                }

                Log.Info("Backup job complete.");
            }
        }

        public Boolean VerifyBackup(Server server, String backupFile)
        {
            Restore restore = new Restore();

            restore.Devices.AddDevice(backupFile, DeviceType.File);

            return restore.SqlVerify(server);
        }

        public void BackupDatabase(Server server, Database database, Configuration configuration, String destination)
        {
            Log.Info("Running a backup for {0} to {1}", database.Name, destination);

            Backup backup = new Backup
            {
                Action = BackupActionType.Database,
                Database = database.Name,
                CompressionOption = configuration.Compression ? BackupCompressionOptions.On : BackupCompressionOptions.Off,
                Incremental = configuration.IncrementalBackup
            };

            backup.Devices.AddDevice(destination, DeviceType.File);

            backup.PercentCompleteNotification = 2;
            backup.PercentComplete += percentCompleteHandler;

            backup.SqlBackup(server);

            Log.Info("Database backup completed.");
        }

        private void percentCompleteHandler(object sender, PercentCompleteEventArgs e)
        {
            Log.Info("Database Backup: {0}% completed.", e.Percent);
        }
    }
}