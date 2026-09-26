using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace EnterpriseServices
{
    public static class DirtbikeSchemaTools
    {
        public static void SchemaDump()
        {
            // Determine project root (bin/Debug/... → project root)
            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

            // Build paths
            string sqlDataDir = Path.Combine(projectRoot, "SQLDATA");
            string dbPath = Path.Combine(sqlDataDir, "dirtbike.db");
            string outputPath = Path.Combine(sqlDataDir, "schemadump.txt");

            if (!File.Exists(dbPath))
            {
                Console.WriteLine("ERROR: Could not find the database file at:");
                Console.WriteLine(dbPath);
                return;
            }

            Console.WriteLine($"Reading schema from: {dbPath}");
            Console.WriteLine($"Writing output to:   {outputPath}");

            try
            {
                using var conn = new SqliteConnection($"Data Source={dbPath}");
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT type, name, tbl_name, sql
                    FROM sqlite_master
                    WHERE sql IS NOT NULL
                    ORDER BY type, name;
                ";

                using var reader = cmd.ExecuteReader();
                using var writer = new StreamWriter(outputPath, false);

                while (reader.Read())
                {
                    
                    string type = reader["type"]?.ToString() ?? ""; 
                    string name = reader["name"]?.ToString() ?? ""; 
                    string sql = reader["sql"]?.ToString() ?? "";

                    writer.WriteLine($"-- {type.ToUpper()}: {name}");
                    writer.WriteLine(sql);
                    writer.WriteLine();
                }

                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading schema:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Schema dump completed.");
        }
    }
}
