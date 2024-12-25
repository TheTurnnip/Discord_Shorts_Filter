using Discord_Shorts_Filter.Logging;
using Discord_Shorts_Filter.Tools;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands;

public class MakePostChannel : IAppCommand
{
    private DiscordSocketClient _client;

    public MakePostChannel(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
        commandBuilder.WithName("make_post_channel");
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Makes a new post channel, or " +
                                       "makes an existing channel a post channel.");
        commandBuilder.AddOption("channel_name", 
                                ApplicationCommandOptionType.String,
                                "The name of the channel to create or add to the post system.",
                                true);

        try
        {
            await _client.GetGuild(guildID).CreateApplicationCommandAsync(commandBuilder.Build());
        }
        catch (HttpException exception) 
        {
            string response = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Logger.Error(response);
        }
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        // Get the name of the channel that should be made a post channel.
        string? postChannelName = command.Data.Options.FirstOrDefault(
            option => option.Name == "channel_name"
            )?.Value.ToString();

        SocketTextChannel? commandChannel = command.Channel as SocketTextChannel;
        SocketGuild? commandGuild = commandChannel?.Guild;

        if (commandGuild == null)
        {
            await command.RespondAsync("This command must be run in a server!");
        }
        
        ulong postChannelId = await CreateFilterChannelAsync(commandGuild, postChannelName);
        
        Logger.Debug("Post Channel ID:" + postChannelId.ToString());
        
        await command.RespondAsync($"Channel {postChannelName} has been made a post channel.");
    }

    private async Task<ulong> CreateFilterChannelAsync(SocketGuild guild, string channelName)
    {
        ValidatedChannelName validatedChannelName = new ValidatedChannelName(channelName);
        
        foreach (SocketGuildChannel channel in guild.Channels)
        {
            if (validatedChannelName.ValidName == channel.Name)
            {
                return channel.Id;
            }
        }

        ITextChannel newChannel = await guild.CreateTextChannelAsync(validatedChannelName.ValidName);
        return newChannel.Id;
    }
}