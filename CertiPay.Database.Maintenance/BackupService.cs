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
    /// Provides functions for conducting backups of a SQL Server database, using optional compression
    /// and verification when complete
    /// </summary>
    public class BackupService
    {
        // TODO Logging right now is tied to CertiPay.Common which uses Serilog. Might pull out that dependency and use Common.Logging?

        private static readonly ILog Log = LogManager.GetLogger<BackupService>();

        public class Configuration
        {
            /// <summary>
            /// The connection string for running the backup, including database name
            /// </summary>
            public String ConnectionString { get; set; }

            /// <summary>
            /// The output file for the backup. Will use SQL Server's configured backup folder
            /// </summary>
            public String OutputFileName { get; set; }

            /// <summary>
            /// True if the backup should be checked for consistency before backup, defaults to TRUE
            /// </summary>
            public Boolean RunCheckDb { get; set; }

            /// <summary>
            /// True if the backup should use compression, defaults to TRUE
            /// </summary>
            public Boolean Compression { get; set; }

            /// <summary>
            /// True if the backup should be an incremental backup, defaults to FALSE
            /// </summary>
            public Boolean IncrementalBackup { get; set; }

            /// <summary>
            /// True if the backup should be verified after completion, defaults to TRUE
            /// </summary>
            public Boolean VerifyBackup { get; set; }

            public Configuration()
            {
                this.RunCheckDb = true;
                this.Compression = true;
                this.VerifyBackup = true;
            }
        }

        /// <summary>
        /// Run a backup with a set of configurable options
        /// </summary>
        public void Run(Configuration configuration)
        {
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection) { StatementTimeout = 0 });
                Database database = new Database(server, connection.Database);

                Log.Info("Starting database backup job");

                // Loads the table/index information
                database.Refresh();

                if (configuration.RunCheckDb)
                {
                    // Check database integrity ~ DBCC CheckDB
                    MaintenanceService.CheckDatabase(database);
                }

                // TODO Allow a configurable backup directory?

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

        /// <summary>
        /// Returns true if the backup file is passed verification on SQL Server
        /// </summary>
        public Boolean VerifyBackup(Server server, String backupFile)
        {
            Restore restore = new Restore();

            restore.Devices.AddDevice(backupFile, DeviceType.File);

            return restore.SqlVerify(server);
        }

        /// <summary>
        /// Backup the database to the configured destination
        /// </summary>
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
            // TODO Should provide a configurable handler for updates on backup progress

            Log.Info("Database Backup: {0}% completed.", e.Percent);
        }
    }
}