using Discord_Shorts_Filter.Logging;
using Microsoft.Extensions.Logging;

namespace Discord_Shorts_Filter.Database;

public class Database
{
    private static Database? _instance = null;
    
    private static Database? Instance 
    { 
        get => _instance; 
        set => _instance = value; 
    }

    private string FilePath { get; set; }
    private string? ConnectionString { get; set; }
    
    private Database(string filePath)
    {
        FilePath = filePath;
        ConnectionString = $"Data Source={filePath}; Version=3; FailIfMissing=True;";
    }

    public static Database GetDatabase(string filePath)
    {
        if (Instance == null)
        {
            Instance = new Database(filePath);
        }
        return Instance;
    }

    private void CreateDatabase()
    {
        
    }
}