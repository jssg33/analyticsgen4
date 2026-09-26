using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace EnterpriseServices
{
    public static class DatabaseTools
    {
        public static void LoadInitData()
        {
            Console.WriteLine("=== Dirtbike SQLite Initialization ===");

            // Determine project root (bin/Debug/... → project root)
            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

            string sqlDataDir = Path.Combine(projectRoot, "SQLDATA");
            string dbPath = Path.Combine(sqlDataDir, "dirtbike.db");
            string sqlFilePath = Path.Combine(sqlDataDir, "initial.sql");

            Console.WriteLine($"SQLDATA directory: {sqlDataDir}");
            Console.WriteLine($"Database path:     {dbPath}");
            Console.WriteLine($"SQL file:          {sqlFilePath}");

            // Ensure SQLDATA directory exists
            if (!Directory.Exists(sqlDataDir))
            {
                Console.WriteLine("Creating SQLDATA directory...");
                Directory.CreateDirectory(sqlDataDir);
            }

            // Ensure initial.sql exists
            if (!File.Exists(sqlFilePath))
            {
                Console.WriteLine("ERROR: initial.sql not found in SQLDATA directory.");
                return;
            }

            // Create DB file if missing
            if (!File.Exists(dbPath))
            {
                Console.WriteLine("Creating new SQLite database file...");
                File.Create(dbPath).Dispose();
            }

            // Load SQL script
            string sqlScript = File.ReadAllText(sqlFilePath);

            try
            {
                using var conn = new SqliteConnection($"Data Source={dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sqlScript;
                cmd.ExecuteNonQuery();

                Console.WriteLine("Database initialization complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR executing SQL script:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
