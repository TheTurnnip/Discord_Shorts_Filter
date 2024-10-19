namespace Discord_Shorts_Filter
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Discord_Shorts_Filter.Logging;
    using Discord_Shorts_Filter.Configuration;
    using Microsoft.Extensions.Configuration;
    using System.Diagnostics;

    internal class Program
    {
        private static DiscordSocketClient? _client;
        private static CommandService? _commandService;
        private static BotConfiguration? _botConfig;

        public static async Task Main(string[] args)
        {

            _botConfig = BotConfiguration.GetBotConfiguration();
            _client = new DiscordSocketClient(_botConfig.Config);
            
            _commandService = new CommandService();

            _client.MessageReceived += _client_MessageReceived;

            await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
            await _client.StartAsync();


            LoggingService loggingService = new LoggingService(_client, _commandService);

            await Task.Delay(-1);
        }

        private static Task _client_MessageReceived(SocketMessage message)
        {
            Logger.Info(message.Content);   
            return Task.CompletedTask;
        }
    }

}
