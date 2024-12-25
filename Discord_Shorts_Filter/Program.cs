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
        private static MakeFilterChannel? _makeFilterChannel;

        public static async Task Main(string[] args)
        {
            _botConfig = BotConfiguration.GetBotConfiguration();
            _client = new DiscordSocketClient(_botConfig.Config);
            _database = Database.Database.GetDatabase(_botConfig.DatabasePath);
            _loggingService = new LoggingService(_client, CommandService);
            _makeFilterChannel = new MakeFilterChannel(_client);

            _client.Ready += _makeFilterChannel.AddCommandAsync;
            _client.SlashCommandExecuted += _makeFilterChannel.HandleCommandAsync;

            await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }
    }

}
