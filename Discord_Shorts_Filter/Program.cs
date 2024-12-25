namespace Discord_Shorts_Filter
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Discord_Shorts_Filter.Logging;
    using Discord_Shorts_Filter.Configuration;
    using Discord_Shorts_Filter.AppCommands;
    using Discord_Shorts_Filter.Tools;
    using Discord_Shorts_Filter.Database;

    internal class Program
    {
        private static DiscordSocketClient? _client;
        private static Database.Database? _database;
        private static readonly CommandService CommandService = new CommandService();
        private static BotConfiguration? _botConfig;
        private static LoggingService? _loggingService;
        private static List<IAppCommand> _appCommands;

        public static async Task Main(string[] args)
        {
            _botConfig = BotConfiguration.GetBotConfiguration();
            _client = new DiscordSocketClient(_botConfig.Config);
            _database = Database.Database.GetDatabase(_botConfig.DatabasePath);
            _loggingService = new LoggingService(_client, CommandService);

            _appCommands = new List<IAppCommand>()
            {
                new MakeFilterChannel(_client),
                new MakePostChannel(_client),
                new DeleteFilterChannel(_client)
            };

            _client.Ready += ClientOnReady;
            
            await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }

        private static async Task ClientOnReady()
        {
            // Register each command with all the guilds the bot is in.
            foreach (SocketGuild guild in _client.Guilds)
            {
                foreach (IAppCommand appCommand in _appCommands)
                {
                    appCommand.AddCommandAsync(guild.Id);
                }
            }

            // Subscribe the client to each command handler.
            foreach (IAppCommand appCommand in _appCommands)
            {
                _client.SlashCommandExecuted += appCommand.HandleCommandAsync;
            }
        }
    }
}