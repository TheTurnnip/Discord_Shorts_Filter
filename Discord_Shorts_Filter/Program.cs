using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Discord_Shorts_Filter.Configuration;
using Discord_Shorts_Filter.AppCommands;
using Discord_Shorts_Filter.Tools;
using Discord_Shorts_Filter.Database;
using Discord_Shorts_Filter.Database.Models;

namespace Discord_Shorts_Filter;

internal class Program
{
    private static DiscordSocketClient? _client;
    private static Database.Database? _database;
    private static readonly CommandService CommandService = new CommandService();
    private static BotConfiguration? _botConfig;
    private static LoggingService? _loggingService;
    private static Logger _applicationLogger;
    private static List<IAppCommand> _appCommands;

    public static async Task Main(string[] args)
    {
        _applicationLogger = Logger.GetLogger("Application Logger", LogLevel.Debug);
        _botConfig = BotConfiguration.GetBotConfiguration();
        _client = new DiscordSocketClient(_botConfig.Config);
        _database = Database.Database.GetDatabase(_botConfig.DatabasePath);
        _loggingService = new LoggingService(_client, CommandService, _applicationLogger);

        _appCommands = new List<IAppCommand>()
        {
            new MakeFilterChannel(_client, _applicationLogger),
            new MakePostChannel(_client, _applicationLogger),
            new RemoveFilterChannel(_client, _applicationLogger),
            new RemovePostChannel(_client, _applicationLogger),
            new FilterAssoiation(_client, _applicationLogger)
        };

        // _client.Ready += ClientOnReady;
        
        _applicationLogger.Info("Logging into Discord...");
        await _client.LoginAsync(TokenType.Bot, _botConfig.Token);
        _applicationLogger.Info("Logged into Discord!");
        _applicationLogger.Info("Starting the bot...");
        await _client.StartAsync();
        _applicationLogger.Info("The bot has been started!");
        
        await Task.Delay(-1);
    }

    private static async Task ClientOnReady()
    {
        _applicationLogger.Info("The bot is ready!");

        // Register each command with all the guilds the bot is in.
        foreach (SocketGuild guild in _client.Guilds)
        {
            _applicationLogger.Info($"Staring to register commands for Guild: {guild.Name} || ID: {guild.Id}");
            foreach (IAppCommand appCommand in _appCommands)
            {
                await appCommand.AddCommandAsync(guild.Id);
            }
        }
            
        // Subscribe the client to each command handler.
        foreach (IAppCommand appCommand in _appCommands)
        {
            _applicationLogger.Info($"Setting up handler for command {appCommand}...");
            _client.SlashCommandExecuted += appCommand.HandleCommandAsync;
            _applicationLogger.Info($"{appCommand.GetType()} has been setup!");
        }
    }
}