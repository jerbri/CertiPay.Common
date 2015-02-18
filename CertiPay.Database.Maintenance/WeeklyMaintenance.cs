﻿namespace CertiPay.Database.Maintenance
{
    using CertiPay.Common.Logging;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Perform routine weekly maintenance on the database
    /// </summary>
    public class WeeklyMaintenance
    {
        private static readonly ILog Log = LogManager.GetLogger<WeeklyMaintenance>();

        public class Configuration
        {
            public String ConnectionString { get; set; }

            /// <summary>
            /// Rebuild indices whose fragmentation is greater than this percentage (defaults to 30%)
            /// </summary>
            public int RebuildIndicesPercentage { get; set; }

            /// <summary>
            /// Reorganize indices whose fragmentation is greater than this percentage (defaults to 10%)
            /// </summary>
            public int ReorganizeIndicesPercentage { get; set; }

            public Configuration()
            {
                this.RebuildIndicesPercentage = 30;
                this.ReorganizeIndicesPercentage = 10;
            }
        }

        public void Execute(Configuration configuration)
        {
            Log.Info("Starting database weekly maintenance.");

            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection) { StatementTimeout = 0 });
                Database database = new Database(server, connection.Database);

                // Loads the table/index information
                database.Refresh();

                // Checks the database for consistency
                CheckDatabase(database);

                // Run maintenace as configured
                IndexMaintenance(database, configuration);

                // Many people actually tell you to NOT shrink your database files
                // ShrinkDatabase(database);
            }

            Log.Info("Completed database weekly maintenance job");
        }

        public static void CheckDatabase(Database database)
        {
            Log.Info("Checking consistency for database {0}", database.Name);

            // Any output messages from CheckTables are bad things

            foreach (var message in database.CheckTables(RepairType.None))
            {
                Log.Fatal(message);
            }

            Log.Info("Finished checking consistency for database {0}", database.Name);
        }

        public void ShrinkDatabase(Database database)
        {
            Log.Info("Performing shrink on database {0}", database.Name);

            database.Shrink(10, ShrinkMethod.TruncateOnly);

            Log.Info("Finished shrink on database {0}", database.Name);
        }

        public void IndexMaintenance(Database database, Configuration configuration)
        {
            Log.Info("Performing maintenance on indices of database {0}", database.Name);

            foreach (Table table in database.Tables)
            {
                Log.Debug("Performing maintenance on indices for table {0}", table.Name);

                foreach (Index index in table.Indexes)
                {
                    Log.Debug("Checking index {0} on {1} for fragmentation", index.Name, table.Name);

                    Decimal fragmentation = GetFragmentation(index);

                    if (fragmentation > configuration.RebuildIndicesPercentage)
                    {
                        Log.Info("Index {0} on {1} requires rebuild, fragmentation {2}%", index.Name, table.Name, fragmentation);

                        // If the fragmentation is high, build the index.
                        // This will also update the statistics along the way

                        index.Rebuild();
                    }
                    else if (fragmentation > configuration.ReorganizeIndicesPercentage)
                    {
                        Log.Info("Index {0} on {1} requires reorganization, fragmentation {2}%", index.Name, table.Name, fragmentation);

                        // If the fragmentation is medium, just organize it.
                        // We will then update the statistics

                        index.Reorganize();

                        index.UpdateStatistics();
                    }
                    else
                    {
                        Log.Debug("Index {0} is healthy, fragmentation {1}%", index.Name, fragmentation);
                    }
                }
            }

            Log.Info("Finished rebuilding indices on database {0}", database.Name);
        }

        private Decimal GetFragmentation(Index index)
        {
            DataTable dt = index.EnumFragmentation();

            return Decimal.Parse(dt.Rows[0]["AverageFragmentation"] + "");
        }
    }
}