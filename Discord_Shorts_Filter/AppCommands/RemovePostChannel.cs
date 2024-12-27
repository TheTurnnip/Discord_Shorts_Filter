using Discord_Shorts_Filter.Logging;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands;

public class RemovePostChannel : IAppCommand
{
    private DiscordSocketClient _client;
    
    private Logger CommandLogger { get; set; } = Logger.GetLogger("RemovePostChannel Logger", LogLevel.Info);

    public RemovePostChannel(DiscordSocketClient client)
    {
        _client = client;
    }

    public RemovePostChannel(DiscordSocketClient client, Logger logger) : this(client)
    {
        CommandLogger = logger;
    }

    public string CommandName { get; } = "remove_post_channel";

    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
        commandBuilder.WithName(CommandName);
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Removes a channel from the post system.");
        commandBuilder.AddOption("post_channel_name",
                                 ApplicationCommandOptionType.String,
                                 "The channel to remove from the post system.",
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
        if (command.CommandName != CommandName)
        {
            return;
        }

        if (command.GuildId == null)
        {
            await command.RespondAsync("This command must be used in a server!", ephemeral: true);
        }
        
        SocketTextChannel? channel = command.Data.Options.First(
            option => option.Name == "filter_channel_id"
        ).Value as SocketTextChannel;

        if (channel == null)
        {
            await command.RespondAsync("There was an error finding the channel in the server.", 
                ephemeral: true);
        }
        
        await command.RespondAsync("Removed channel from the post system. " +
                                   "\nYou can now safely delete it from the server.", 
                                    ephemeral: true);
    }
}