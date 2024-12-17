namespace Discord_Shorts_Filter
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Discord_Shorts_Filter.Logging;
    using Discord_Shorts_Filter.Configuration;
    using Discord_Shorts_Filter.AppCommands;

    internal class Program
    {
        private static DiscordSocketClient? _client;
        private static CommandService _commandService = new CommandService();
        private static BotConfiguration? _botConfig;
        private static LoggingService? _loggingService;
        private static MakeFilterChannel? _makeFilterChannel;

        public static async Task Main(string[] args)
        {

            _botConfig = BotConfiguration.GetBotConfiguration();
            _client = new DiscordSocketClient(_botConfig.Config);
            _loggingService = new LoggingService(_client, _commandService);
            _makeFilterChannel = new MakeFilterChannel(_client);

            _client.Ready += _makeFilterChannel.AddCommandAsync;
            _client.SlashCommandExecuted += _makeFilterChannel.CommandHandler;

            await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task AddCommands()
        {
            if (_client == null)
            {
                string message = "Can't add commands to a null client!";
                Logger.Error(message);
                throw new NullReferenceException(message);
            }
            await _makeFilterChannel.AddCommandAsync();
        }
    }

}
