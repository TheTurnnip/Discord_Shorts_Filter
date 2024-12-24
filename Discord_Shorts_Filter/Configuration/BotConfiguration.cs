using Discord;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Microsoft.Extensions.Configuration;

namespace Discord_Shorts_Filter.Configuration
{
    internal sealed class BotConfiguration
    {
        private static BotConfiguration? Instance { get; set; }
        internal DiscordSocketConfig? Config { private set; get; }
        internal string? Token { private set; get; }
        internal string? DatabasePath { private set; get; }

        private BotConfiguration() 
        {
            // Reads in the token from the .NET User Secrets. Used for building in Visual Studio.
            IConfiguration secretsConfiguration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretsToken;

            try
            {
                secretsToken = secretsConfiguration.GetRequiredSection("Discord")["TOKEN"];
            }
            catch (InvalidOperationException)
            {
                // Sets the token to null if the .net secrets file can't be read.
                secretsToken = null;
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

            Config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                LogGatewayIntentWarnings = true,
                UseInteractionSnowflakeDate = false
            };
            
            // Set path to the sqlite database.
            DatabasePath = Environment.GetEnvironmentVariable("FILTER_DB_PATH") ?? "/shorts_filter_db/filters.db";
        }

        internal static BotConfiguration GetBotConfiguration() 
        {
            if (Instance == null)
            {
                Instance = new BotConfiguration();
            }

            return Instance;
        }
    }
}
