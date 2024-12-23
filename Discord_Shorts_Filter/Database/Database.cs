using Discord_Shorts_Filter.Logging;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Discord_Shorts_Filter.Database;

/// <summary>
/// Represents the database that is used to persist data for the bot.
/// </summary>
public class Database
{
	/// <summary>
	/// Gets and Sets the instance of the database class.
	/// </summary>
	private static Database? Instance { get; set; } = null;
	
	/// <summary>
	/// Gets and Sets the file path to where the sqlite database file is.
	/// </summary>
    private string FilePath { get; set; }
	
	/// <summary>
	/// The connection string that is used to connect to the sqlite database.
	/// </summary>
    private string? ConnectionString { get; set; }
    
	/// <summary>
	/// Creates an instance of the Database class using the path
	/// to the database file.
	/// </summary>
	/// <param name="filePath">The path to the sqlite database file.</param>
    private Database(string filePath)
    {
        FilePath = filePath;
        ConnectionString = $"Data Source={filePath};";
        if (!CreateDatabase())
        {
	        Logger.Error("There was an error creating the database!");
        }
    }

	/// <summary>
	/// Gets the instance of the Database.
	/// </summary>
	/// <param name="filePath">
	/// The path to where the sqlite database file
	/// is located.
	/// </param>
	/// <returns>The instance of the database object.</returns>
    public static Database GetDatabase(string filePath)
    {
	    return Instance ??= new Database(filePath);
    }
    
	/// <summary>
	/// Create the tables that the bot needs in the sqlite database.
	/// </summary>
	/// <returns>True if the database was created, false if there was an error.</returns>
    private bool CreateDatabase()
    {
	    if (!CreateDatabaseFile()) return false;

	    using var connection = new SqliteConnection(ConnectionString);
	    connection.Open();
            
	    SqliteCommand createDatabaseCommand = connection.CreateCommand();
	    createDatabaseCommand.CommandText = 
		    @"
				CREATE TABLE IF NOT EXISTS YoutubeChannels(
					id INT PRIMARY KEY NOT NULL,
					channel_name TEXT,
					channel_handle TEXT UNIQUE NOT NULL,
					channel_url TEXT UNIQUE NOT NULL
				);

				CREATE TABLE IF NOT EXISTS DiscordFilterChannels(
					id INT PRIMARY KEY NOT NULL,
					channel_id INT UNIQUE NOT NULL,
					channel_name TEXT
				);

				CREATE TABLE IF NOT EXISTS DiscordPostChannels(
					id INT PRIMARY KEY NOT NULL,
					channel_id INT UNIQUE NOT NULL,
					channel_name INT NOT NULL
				);

				CREATE TABLE IF NOT EXISTS DiscordFilterChannelsDiscordPostChannels(
					id INT PRIMARY KEY NOT NULL,
					filter_channel_id INT NOT NULL,
					post_channel_name INT NOT NULL,
					
					FOREIGN KEY (filter_channel_id) REFERENCES DiscordFilterChannels (id) ON DELETE SET NULL,
					FOREIGN KEY (post_channel_name) REFERENCES DiscordPostChannels (id) ON DELETE SET NULL 
				);

				CREATE TABLE IF NOT EXISTS YoutubeChannelsDiscordFilterChannels(
					id PRIMARY KEY NOT NULL,
					youtube_channel_id INT NOT NULL,
					filter_channel_id INT NOT NULL,
					
					FOREIGN KEY (youtube_channel_id) REFERENCES YoutubeChannels (id) ON DELETE SET NULL,
					FOREIGN KEY (filter_channel_id) REFERENCES DiscordFilterChannels (id) ON DELETE SET NULL 
				);
			";
	    
	    createDatabaseCommand.ExecuteNonQuery();
	    return true;
    }

	/// <summary>
	/// Creates the path to the sqlite file, and the database
	/// file if it does not exist.
	/// </summary>
	/// <returns>True if the file could be created, false if there was an error.</returns>
    private bool CreateDatabaseFile()
    {
	    if (!File.Exists(FilePath))
	    {
		    try
		    {
			    string? path = Path.GetDirectoryName(FilePath);
			    if (path != null)
			    {
				    Directory.CreateDirectory(path);
			    }
			    File.Create(FilePath).Close();
		    }
		    catch (Exception e)
		    {
			    Logger.Error(e.Message);
			    return false;
		    }
	    }
	    return true;
    }
}