using Discord;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Microsoft.Extensions.Configuration;

namespace Discord_Shorts_Filter.Configuration
{
    /// <summary>
    /// Represents the discord bots configuration.
    /// </summary>
    internal sealed class BotConfiguration
    {
        /// <summary>
        /// Represents the instance of the BotConfiguration class.
        /// </summary>
        private static BotConfiguration? Instance { get; set; }
        
        /// <summary>
        /// The DiscordSocketConfig that stores lower level bot configuration.
        /// </summary>
        internal DiscordSocketConfig? Config { private set; get; }
        
        /// <summary>
        /// The discord token that is used to run the bot.
        /// </summary>
        internal string? Token { private set; get; }

        /// <summary>
        /// The path to where the sqlite used by the bot should be created
        /// if it does not exist, and stored.
        /// </summary>
        internal string? DatabasePath { private set; get; }

        /// <summary>
        /// Creates an instance of the BotConfiuration class by getting configuration
        /// details from evniroment variables or .NET Secrets.
        /// </summary>
        private BotConfiguration() 
        {
            // Reads in the token from the .NET User Secrets. Used for building in Visual Studio.
            IConfiguration secretsConfiguration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretsToken = null;
            
            try
            {
                secretsToken = secretsConfiguration.GetRequiredSection("Discord")["TOKEN"];
            }
            catch (InvalidOperationException)
            {
                // Sets the token to null if the .net secrets file can't be read.
                Logger.Debug(".NET Secrets not found. Most users can ignore this message..." +
                             "\n If you are a dev check your config.");
            }

            // Gets the token when running in the docker container.
            string? envToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            // Sets the token to be which ever token is not null.
            Token = envToken ?? secretsToken;

            if (Token == null)
            {
                Logger.Error("There was an error locating the bot token. Please ensure that it was set.");
            }

            /*
             * Sets some lower level settings for the discord bot.
             * GatewayIntents: Limits what can be sent from discord to the bot.
             * LogGatewayIntentWarnings: If Intent warnings are logged.
             * UseInteractionSnowflakeDate: If the system time should be used to check dates.
             */
            Config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                LogGatewayIntentWarnings = true,
                UseInteractionSnowflakeDate = false
            };
            
            // Set path to the sqlite database.
            DatabasePath = Environment.GetEnvironmentVariable("FILTER_DB_PATH") ?? "/shorts_filter_db/filters.db";
        }
        
        /// <summary>
        /// Gets the instance of the bot configuration.
        /// </summary>
        /// <returns>
        /// Returns a new instance of the bot config if one does not already exist,
        /// if one does already exist it returns that.
        /// </returns>
        internal static BotConfiguration GetBotConfiguration()
        {
            return Instance ??= new BotConfiguration();
        }
    }
}
