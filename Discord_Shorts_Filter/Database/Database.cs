using Discord_Shorts_Filter.Logging;
using Dapper;
using Discord_Shorts_Filter.Database.Models;
using Discord;
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
	/// The Logger that is to be used for the Database class.
	/// </summary>
	private Logger DatabaseLogger { get; set; } = Logger.GetLogger("Database Logger", LogLevel.Info);
    
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
	        DatabaseLogger.Error("There was an error creating the database!");
        }
    }

	private Database(string filename, Logger databaseLogger) : this(filename) 
	{
		DatabaseLogger = databaseLogger;
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

	public static Database GetDatabase(string filePath, Logger databaseLogger)
	{
		return Instance ??= new Database(filePath, databaseLogger);
	}
    
	/// <summary>
	/// Create the tables that the bot needs in the sqlite database.
	/// </summary>
	/// <returns>True if the database was created, false if there was an error.</returns>
    private bool CreateDatabase()
    {
	    if (!CreateDatabaseFile())
	    {
		    DatabaseLogger.Error("Unable to create the database tables due to error" +
		                         "when creating the sqlite database file.");
		    return false;
	    }

	    using var connection = new SqliteConnection(ConnectionString);
	    connection.Open();
            
	    SqliteCommand createDatabaseCommand = connection.CreateCommand();
	    createDatabaseCommand.CommandText = 
		    @"
				CREATE TABLE IF NOT EXISTS DiscordFilterChannels(
					ChannelId INT UNIQUE NOT NULL,
					GuildId INT UNIQUE NOT NULL,
					ChannelName TEXT NOT NULL
				);

				CREATE TABLE IF NOT EXISTS DiscordPostChannels(
					ChannelId INT UNIQUE NOT NULL,
					GuildId INT UNIQUE NOT NULL,
					ChannelName INT NOT NULL
				);

				CREATE TABLE IF NOT EXISTS FilterPostMaps(
					FilterChannelId INT NOT NULL,
					PostChannelId INT NOT NULL,
					
					FOREIGN KEY (FilterChannelID) REFERENCES DiscordFilterChannels (ChannelId) ON DELETE SET NULL,
					FOREIGN KEY (PostChannelID) REFERENCES DiscordPostChannels (ChannelId) ON DELETE SET NULL 
				);
			";

	    try
	    {
		    createDatabaseCommand.ExecuteNonQuery();
		    return true;
	    }
	    catch (Exception e)
	    {
		    DatabaseLogger.Critical("An Error was encoutnered when trying to " +
		                            "create the database tables. The program will now exit...");
		    throw new IOException("There was an error comunicating with the database!");
	    }
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
		    DatabaseLogger.Info("Creating the database file...");
		    try
		    {
			    string? path = Path.GetDirectoryName(FilePath);
			    if (path != null)
			    {
				    Directory.CreateDirectory(path);
			    }
			    File.Create(FilePath).Close();
			    DatabaseLogger.Info("The database file has been created.");
		    }
		    catch (Exception e)
		    {
			    DatabaseLogger.Error("There was an error making the database file.");
			    DatabaseLogger.Critical(e.Message);
			    return false;
		    }
	    }
	    else
	    {
		    DatabaseLogger.Info("The database file already exists, skipping creation...");
	    }
	    return true;
    }

	/// <summary>
	/// Gets a Discord channel from the database
	/// and returns it's model.
	/// </summary>
	/// <param name="channelId">The discord channel's ID</param>
	/// <typeparam name="T">
	/// The model that you want to select from the database.
	/// The model must implement the IDiscordChannel Interface
	/// and have a constructor that takes no parameters.
	/// </typeparam>
	/// <returns>An IDiscordChannel model of type T</returns>
	public T GetDiscordChannel<T>(ulong channelId) where T : IDiscordChannel, new()
	{
		using var connection = new SqliteConnection(ConnectionString);
		string tableName = typeof(T).Name;
		
		string query = $"SELECT * FROM [{tableName}] WHERE ChannelId = @Id";
		
		T output = connection.QuerySingle<T>(query, new { Id = channelId });
		return output;
	}

	/// <summary>
	/// Insert a Discord channel into the Database
	/// </summary>
	/// <param name="channelId">The discord channel ID</param>
	/// <param name="guildId">The discord server ID</param>
	/// <param name="channelName">The name of the discord channel.</param>
	/// <typeparam name="T">
	/// The model class for the table to insert the data into.
	/// The model class must implement the IDiscordChannel interface and
	/// have a constructor that takes no parameters.
	/// </typeparam>
	/// <returns>True of the insert was sucessful, and false if it failed.</returns>
	public bool InsertDiscordChannel<T>(ulong channelId, ulong guildId, string channelName) where T : IDiscordChannel, new()
	{
		using var connection = new SqliteConnection(ConnectionString);
		string tableName = typeof(T).Name;
		
		string query = $"INSERT INTO [{tableName}] (ChannelId, GuildId, ChannelName) VALUES (@ChannelId, @GuildId, @ChannelName)";
		
		try
		{ 
			connection.Execute(query, new { ChannelId = channelId, GuildId = guildId, ChannelName = channelName });
		}
		catch (Exception)
		{
			DatabaseLogger.Critical($"There was an error inserting the Discord channel into table: {tableName}.");
			return false;
		}
		return true;
	}

	/// <summary>
	/// Delete a Discord channel from the database.
	/// </summary>
	/// <param name="channelId">The ID of the discord channel to delete.</param>
	/// <typeparam name="T">
	/// The model class for the table to delete the data from.
	/// The model class must implement the IDiscordChannel interface and
	/// have a constructor that takes no parameters.
	/// </typeparam>
	/// <returns>True if the delete was successful, false if it failed.</returns>
	public bool DeleteDiscordChannel<T>(ulong channelId) where T : IDiscordChannel, new()
	{
		using var connection = new SqliteConnection(ConnectionString);
		string tableName = typeof(T).Name;
		
		string query = $"DELETE FROM [{tableName}] WHERE ChannelId = @ChannelId";

		try
		{
			connection.Execute(query, new { ChannelId = channelId });
		}
		catch (Exception)
		{
			DatabaseLogger.Critical($"There was an error deleting the Discord channel from table: {tableName}.");
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Update a Discord channel in the database.
	/// </summary>
	/// <param name="currentChannelId">
	/// The ID of the disocrd channel that
	/// is currently in the database.
	/// </param>
	/// <param name="newChannelId">
	/// The new Discord channel ID to update the row with.
	/// </param>
	/// <param name="guildId">
	/// The server ID that the the row should be updated with.
	/// </param>
	/// <param name="channelName">
	/// The new channel name that the row should be updated with.
	/// </param>
	/// <typeparam name="T">
	/// The model class for the table to update the data for.
	/// The model class must implement the IDiscordChannel interface and
	/// have a constructor that takes no parameters.
	/// </typeparam>
	/// <returns>True if the update was successful, false if it failed.</returns>
	public bool UpdateDiscordChannel<T>(ulong currentChannelId, ulong newChannelId, ulong guildId, string channelName) where T : IDiscordChannel, new()
	{
		using var connection = new SqliteConnection(ConnectionString);
		string tableName = typeof(T).Name;

		string query = $"UPDATE [{tableName}] SET ChannelId = @ChannelId, GuildId = @GuildId, ChannelName = @ChannelName WHERE ChannelId = {currentChannelId};";

		try
		{
			connection.Execute(query, new { ChannelId = newChannelId, GuildId = guildId, ChannelName = channelName });
		} catch (Exception)
		{
			DatabaseLogger.Critical("There was an error updating table: {tableName}.");
			return false;
		}
		return true;
	}
}