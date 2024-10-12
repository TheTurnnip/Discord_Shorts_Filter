namespace Discord_Shorts_Filter
{

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Discord_Shorts_Filter.Logging;
    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        private static DiscordSocketClient? _client;
        private static CommandService? _commandService;

        public static async Task Main(string[] args)
        {

            string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            if (token == null)
            {
                throw new NullReferenceException("The token was null!");
            }

            Logger.Info(token);

            _client = new DiscordSocketClient();
            _commandService = new CommandService();

            try
            {
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            LoggingService loggingService = new LoggingService(_client, _commandService);

            await Task.Delay(-1);
        }
    }
}
