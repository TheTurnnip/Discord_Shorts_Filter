using Discord_Shorts_Filter.Database.Models;
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

    private Database.Database Database { get; set; }
    
    public RemoveFilterChannel(DiscordSocketClient client, Database.Database database)
    {
        _client = client;
        Database = database;
    }

    public RemoveFilterChannel(DiscordSocketClient client, Database.Database database, Logger commandLogger) : this(client, database)
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
        SocketChannel? channel = command.Data.Options.First(
            option => option.Name == "filter_channel_id"
        ).Value as SocketChannel;

        if (channel == null)
        {
            await command.RespondAsync("There was an error finding the channel in the server.", 
                ephemeral: true);
        }

        switch (channel.GetChannelType())
        {
            case ChannelType.Text:
                if (!await RemoveChannelAsync(channel))
                {
                    await command.RespondAsync("There was an error removing the channel from the systme.", 
                                                ephemeral: true);
                }
                else
                {
                    await command.RespondAsync("The channel has been removed from the system. " +
                                               "\nYou may safely delete the channel in Discord.", 
                                                ephemeral: true);
                }
                break;
            case ChannelType.Category:
                List<string> errors = await RemoveCategoryAsync(channel, (ulong)command.GuildId);
                if (errors.Count > 0)
                {
                    string formattedErrors = "";
                    foreach (var error in errors)
                    {
                        formattedErrors += $"{error}\n";
                    }
                    
                    await command.RespondAsync("Unable to remove some channels from the system. " +
                                               $"Could not remove \n{formattedErrors}", ephemeral: true);
                }
                else
                {
                    await command.RespondAsync("The channel has been removed from the system. " +
                                               "\nYou may safely delete the channel in Discord.", 
                                                ephemeral: true);
                }
                break;
        }
    }

    private async Task<bool> RemoveChannelAsync(SocketChannel channel)
    {
        return Database.DeleteDiscordChannel<DiscordFilterChannels>(channel.Id);
    }

    private async Task<List<string>> RemoveCategoryAsync(SocketChannel channel, ulong guildId)
    {
        List<string> errors = new List<string>();
        SocketCategoryChannel category = _client.GetGuild(guildId).GetCategoryChannel(channel.Id);

        foreach (SocketGuildChannel categoryChannel in category.Channels)
        {
            if (!Database.DeleteDiscordChannel<DiscordFilterChannels>(categoryChannel.Id))
            {
                errors.Add($"Could not channel from system. Name: {categoryChannel.Name}, ID: {categoryChannel.Id}.");
            }
        }
        return errors;
    }
}