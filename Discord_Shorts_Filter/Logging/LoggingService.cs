using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Shorts_Filter.Logging
{
    /// <summary>
    /// Represents the logging service that handles log events from the bot.
    /// </summary>
    public class LoggingService
    {

        /// <summary>
        /// The current date-time.
        /// </summary>
        private readonly string _currentDateTime = DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");

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
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
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
                Console.WriteLine($"{message.Severity} || {_currentDateTime} || Command: " +
                    $"{commandException.Command.Aliases.First()} || Message: {message}");
            }
            else
            {
                Console.WriteLine($"{message.Severity} || {_currentDateTime} || Message: {message}");
            }

            return Task.CompletedTask;
        }
    }
}
