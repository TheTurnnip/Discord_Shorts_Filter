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
    private Logger CommandLogger { get; set; } = Logger.GetLogger("MakePostChannel Logger", LogLevel.Info);

    private Database.Database Database { get; set; }
    
    public MakePostChannel(DiscordSocketClient client, Database.Database database)
    {
        _client = client;
        Database = database;
    }

    public MakePostChannel(DiscordSocketClient client, Database.Database database, Logger commandLogger) : this(client, database)
    {
        CommandLogger = commandLogger;
    }

    public string CommandName { get; } = "make_post_channel";

    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
        commandBuilder.WithName(CommandName);
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
        
        // Get the name of the channel that should be made a post channel.
        string? postChannelName = command.Data.Options.FirstOrDefault(
            option => option.Name == "channel_name"
            )?.Value.ToString();

        SocketTextChannel? commandChannel = command.Channel as SocketTextChannel;
        SocketGuild? commandGuild = commandChannel?.Guild;

        if (commandGuild == null)
        {
            await command.RespondAsync("This command must be run in a server!", 
                                        ephemeral: true);
        }
        
        ulong postChannelId = await CreateFilterChannelAsync(commandGuild, postChannelName);

        CommandLogger.Info($"Created post channel. Guild ID: {command.GuildId} Channel ID: {postChannelId}");
        await command.RespondAsync($"Channel {postChannelName} has been made a post channel.", 
                                    ephemeral: true);
    }

    private async Task<ulong> CreateFilterChannelAsync(SocketGuild guild, string channelName)
    {
        ValidatedChannelName validatedChannelName = new ValidatedChannelName(channelName);
        
        foreach (SocketGuildChannel channel in guild.Channels)
        {
            if (validatedChannelName.ValidName == channel.Name)
            {
                CommandLogger.Info($"Channel {validatedChannelName.ValidName} already exists, skipping creation...");
                return channel.Id;
            }
        }

        ITextChannel newChannel = await guild.CreateTextChannelAsync(validatedChannelName.ValidName);
        CommandLogger.Info($"Created channel: {validatedChannelName} || " +
                           $"Channel ID: {newChannel.Id} || " +
                           $"Server Name: {guild.Name} || " +
                           $"Server ID: {guild.Id}.");
        return newChannel.Id;
    }
}