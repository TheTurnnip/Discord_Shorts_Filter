using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Shorts_Filter.Logging
{
    public class LoggingService
    {

        private readonly string _currentDateTime = DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");

        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }

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
