using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Shorts_Filter.Logging;

/// <summary>
/// Represents the logging service that handles log events from the bot.
/// </summary>
public class LoggingService
{

    private Logger LoggingServiceLogger { get; set; }
        
    /// <summary>
    /// Creates an instance of the Logging Service and subscribes
    /// a client and command service to LogAsync method.
    /// </summary>
    /// <param name="client">
    /// The bot client that the LoggingService will handle events for.
    /// </param>
    /// <param name="command">
    /// The CommandService that the LoggingService will handle events for.
    /// </param>
    public LoggingService(DiscordSocketClient client, CommandService command, Logger logger)
    {
        LoggingServiceLogger = logger;
        client.Log += LogAsync;
        command.Log += LogAsync;
    }

    /// <summary>
    /// Handles Log event from the DiscordSocketClient or CommandService class.
    /// </summary>
    /// <param name="message">The message that has been sent by an event.</param>
    /// <returns>A task representing the compeltion of hadling logging a message.</returns>
    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException commandException)
        {
            string commandLogMessage = $"Command: {commandException.Command.Name} " +
                                       $"Message: {message.Message} " +
                                       $"Exception: {commandException}";
            LogMessage(message.Severity, commandLogMessage);
        }
        else
        {
            LogMessage(message.Severity, message.Message);
        }

        return Task.CompletedTask;
    }

    private void LogMessage(LogSeverity logSeverity, string message)
    {
        switch (logSeverity)
        {
            case LogSeverity.Critical:
                LoggingServiceLogger.Critical(message);
                break;
            case LogSeverity.Error: 
                LoggingServiceLogger.Error(message);
                break;
            case LogSeverity.Warning:
                LoggingServiceLogger.Warn(message);
                break;
            case LogSeverity.Info:
                LoggingServiceLogger.Info(message);
                break;
            case LogSeverity.Verbose:
                LoggingServiceLogger.Verbose(message);
                break;
            case LogSeverity.Debug:
                LoggingServiceLogger.Debug(message);
                break;
        }
    }
}