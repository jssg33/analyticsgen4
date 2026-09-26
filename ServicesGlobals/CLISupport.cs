using Enterprise.Models;

namespace EnterpriseServices
{
    
public static class TwoFactorHelper
{

    public static int GenerateSixDigitRandom()
    {
                Random rnd = new Random();
                int dice = rnd.Next(100000, 1000000);
                return dice;
    }

}
public static class SystemCLISupport
{
    private static EnterpriseContext _context = new EnterpriseContext();

    public static void DumpSchema()
    {
        EnterpriseServices.DirtbikeSchemaTools.SchemaDump();
        Console.WriteLine("Schema dumped to /SQLDATA.");
    }

public static void ProcessNCParks()
{
    Console.WriteLine("Processing NC Parks...");

    try
    {
        // Determine project root (bin/Debug/... → project root)
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

        string dataDir = Path.Combine(projectRoot, "Data");
        string sqlFile = Path.Combine(dataDir, "ncparkswithguid.sql");

        if (!File.Exists(sqlFile))
        {
            Console.WriteLine("ERROR: ncparkswithguid.sql not found in /Data directory.");
            return;
        }

        string sqlScript = File.ReadAllText(sqlFile);

        // Path to your SQLite DB
        string sqlDataDir = Path.Combine(projectRoot, "SQLDATA");
        string dbPath = Path.Combine(sqlDataDir, "dirtbike.db");

        if (!File.Exists(dbPath))
        {
            Console.WriteLine("ERROR: dirtbike.db not found in /SQLDATA directory.");
            return;
        }

        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sqlScript;
        cmd.ExecuteNonQuery();

        Console.WriteLine("NC Parks processed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR processing NC parks:");
        Console.WriteLine(ex.Message);
    }
}


 public static void ProcessVAParks()
{
    Console.WriteLine("Processing VA Parks...");

    try
    {
        // Determine project root (bin/Debug/... → project root)
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

        string dataDir = Path.Combine(projectRoot, "Data");
        string sqlFile = Path.Combine(dataDir, "vaparkswithguid.sql");

        if (!File.Exists(sqlFile))
        {
            Console.WriteLine("ERROR: vaparkswithguid.sql not found in /Data directory.");
            return;
        }

        string sqlScript = File.ReadAllText(sqlFile);

        // Path to your SQLite DB
        string sqlDataDir = Path.Combine(projectRoot, "SQLDATA");
        string dbPath = Path.Combine(sqlDataDir, "dirtbike.db");

        if (!File.Exists(dbPath))
        {
            Console.WriteLine("ERROR: dirtbike.db not found in /SQLDATA directory.");
            return;
        }

        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = sqlScript;
        cmd.ExecuteNonQuery();

        Console.WriteLine("VA Parks processed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR processing VA parks:");
        Console.WriteLine(ex.Message);
    }
}


public static void ProcessAllParks()
{
    Console.WriteLine("Processing ALL parks...");

    try
    {
        // Determine project root (bin/Debug/... → project root)
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

        string queueDir = Path.Combine(projectRoot, "IOQUEUE");

        if (!Directory.Exists(queueDir))
        {
            Console.WriteLine("ERROR: IOQUEUE directory not found: " + queueDir);
            return;
        }

        // Get all .sql files
        string[] sqlFiles = Directory.GetFiles(queueDir, "*.sql");

        if (sqlFiles.Length == 0)
        {
            Console.WriteLine("No SQL files found in IOQUEUE.");
            return;
        }

        // Path to SQLite DB
        string sqlDataDir = Path.Combine(projectRoot, "SQLDATA");
        string dbPath = Path.Combine(sqlDataDir, "dirtbike.db");

        if (!File.Exists(dbPath))
        {
            Console.WriteLine("ERROR: dirtbike.db not found in /SQLDATA directory.");
            return;
        }

        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        foreach (var file in sqlFiles)
        {
            string fileName = Path.GetFileName(file);
            Console.WriteLine($"Processing: {fileName}");

            try
            {
                string sqlScript = File.ReadAllText(file);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sqlScript;
                cmd.ExecuteNonQuery();

                Console.WriteLine($"SUCCESS: {fileName}");
            }
            catch (Exception exFile)
            {
                Console.WriteLine($"ERROR processing {fileName}: {exFile.Message}");
            }
        }

        Console.WriteLine("All park SQL files processed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR in ProcessAllParks:");
        Console.WriteLine(ex.Message);
    }
}


  public static void ShowFileList()
{
    Console.WriteLine("Listing files in /Data ...");

    try
    {
        // Determine project root (bin/Debug/... → project root)
        string baseDir = AppContext.BaseDirectory;
        string projectRoot = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.FullName;

        string dataDir = Path.Combine(projectRoot, "IOQUEUES");

        if (!Directory.Exists(dataDir))
        {
            Console.WriteLine("Data directory not found: " + dataDir);
            return;
        }

        string[] files = Directory.GetFiles(dataDir);

        if (files.Length == 0)
        {
            Console.WriteLine("No files found in /Data.");
            return;
        }

        foreach (var file in files)
        {
            Console.WriteLine(" - " + Path.GetFileName(file));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error listing files: " + ex.Message);
    }
}


    public static void RemoveZeroCarts(int userId)
    {
        //var service = new ZeroCartService(_context);
        //string result = service.ZeroCartUpdate(userId.ToString());
        //Console.WriteLine($"Zero carts removed for user {userId}: {result}");
    }

    public static void UpdateParkAvg(int parkId)
    {
        //var service = new ParkRatingService(_context);
        //string result = service.UpdateAverageParkRating(parkId);
        //Console.WriteLine($"Average rating updated for park {parkId}: {result}");
    }

    public static void UpdateAllParkAvgs()
    {
        //var service = new ParkRatingService(_context);
        //string result = service.UpdateAverageRatingsForFirst500();
        //Console.WriteLine("Average ratings updated for first 500 parks.");
    }
}

}
