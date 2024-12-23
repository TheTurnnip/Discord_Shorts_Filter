using Discord_Shorts_Filter.Logging;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Discord_Shorts_Filter.Database;

public class Database
{
	private static Database? Instance { get; set; } = null;
    private string FilePath { get; set; }
    private string? ConnectionString { get; set; }
    
    private Database(string filePath)
    {
        FilePath = filePath;
        ConnectionString = $"Data Source={filePath};";
        if (!CreateDatabase())
        {
	        Logger.Error("There was an error creating the database!");
        }
    }

    public static Database GetDatabase(string filePath)
    {
	    return Instance ??= new Database(filePath);
    }

    private bool CreateDatabase()
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
}