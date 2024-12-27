using Discord_Shorts_Filter.Logging;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands;

public class RemoveFilterChannel : IAppCommand
{
    private DiscordSocketClient _client;
    private Logger CommandLogger { get; set; } = Logger.GetLogger("RemoveFilterChannel Logger", LogLevel.Info);

    public RemoveFilterChannel(DiscordSocketClient client)
    {
        _client = client;
    }

    public RemoveFilterChannel(DiscordSocketClient client, Logger commandLogger) : this(client)
    {
        CommandLogger = commandLogger;
    }

    public string CommandName { get; } = "remove_filter_channel";

    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();

        commandBuilder.WithName(CommandName);
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Remove a channel from the filter channels system.");
        commandBuilder.AddOption("filter_channel_id", 
                                ApplicationCommandOptionType.Channel, 
                                "The channel or category to remove from the system.",
                                true);
        try
        {            
            await _client.GetGuild(guildID).CreateApplicationCommandAsync(commandBuilder.Build());
            CommandLogger.Info($"Added {CommandName} to {_client.GetGuild(guildID).Name}.");
        }
        catch (HttpException exception) 
        {
            string response = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            CommandLogger.Error(response);
        }
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        // Only allow for the delete_filter_channel command to run.
        if (command.CommandName != CommandName)
        {
            return;
        }
        
        if (command.GuildId == null)
        {
            await command.RespondAsync("This command must be used in a server!");
        }

        // Get the channel that needs to be removed from the system.
        SocketTextChannel? channel = command.Data.Options.First(
            option => option.Name == "filter_channel_id"
        ).Value as SocketTextChannel;

        if (channel == null)
        {
            await command.RespondAsync("There was an error finding the channel in the server.", 
                ephemeral: true);
        }
        
        await command.RespondAsync("Removed channel from the filter system. " +
                                   "\nYou can now safely delete it from the server.", 
                                    ephemeral: true);
    }
}