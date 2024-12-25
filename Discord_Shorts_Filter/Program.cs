using System.Reflection;

namespace Discord_Shorts_Filter
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Discord_Shorts_Filter.Logging;
    using Discord_Shorts_Filter.Configuration;
    using Discord_Shorts_Filter.Tools;
    using Discord_Shorts_Filter.Database;
    using Discord_Shorts_Filter.AppCommands;

    internal class Program
    {
        private static DiscordSocketClient? _client;
        private static Database.Database? _database;
        private static readonly CommandService CommandService = new CommandService();
        private static BotConfiguration? _botConfig;
        private static LoggingService? _loggingService;
        private static List<IAppCommand>? _appCommands;

        public static async Task Main(string[] args)
        {
            _botConfig = BotConfiguration.GetBotConfiguration();
            _client = new DiscordSocketClient(_botConfig.Config);
            if (_botConfig.DatabasePath == null)
            {
                throw new NullReferenceException("The database path cannot be null!");
            }
            _database = Database.Database.GetDatabase(_botConfig.DatabasePath);
            _loggingService = new LoggingService(_client, CommandService);
            
            _appCommands = new List<IAppCommand>()
            {
                new MakeFilterChannel(_client),
                new DeleteFilterChannel(_client)
            };
            await SetupCommands();
            
            await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }
        
        private static Task SetupCommands()
        {
            if (_appCommands == null)
            {
                throw new NullReferenceException("The app commands list can not be null!");
            }
            foreach (IAppCommand appCommand in _appCommands)
            {
                if (_client == null)
                {
                    throw new NullReferenceException("Uanble to setup commands for null client!");
                }
                _client.Ready += appCommand.AddCommandAsync;
                _client.SlashCommandExecuted += appCommand.HandleCommandAsync;
            }

            return Task.CompletedTask;
        }

    }

}
