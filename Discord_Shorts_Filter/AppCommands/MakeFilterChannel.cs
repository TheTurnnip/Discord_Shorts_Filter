using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Discord_Shorts_Filter.Tools;

namespace Discord_Shorts_Filter.AppCommands;

/// <summary>
/// A class that represents the make_filter_channel command, that is used when filtering videos.
/// </summary>
internal class MakeFilterChannel : IAppCommand
{
    /// <summary>
    /// The client that this command has been added to.
    /// </summary>
    private DiscordSocketClient _client;
        
    private Logger CommandLogger { get; set; } = Logger.GetLogger("MakeFilterChannel Logger", LogLevel.Info);

    /// <summary>
    /// Name of the option for adding a filter channel.
    /// </summary>
    private static readonly string optionChannelName = "channel_name";
        
    /// <summary>
    /// The default name of the filter channel.
    /// </summary>
    private static readonly string defaultChannelName = "filter_channel";
        
    /// <summary>
    /// Name of the option for adding a category for filter channels.
    /// </summary>
    private static readonly string optionChannelCategoryName = "category_name";

    /// <summary>
    /// The default name for the filter channel category.
    /// </summary>
    private static readonly string defaultChannelCategoryName = "Filters";

    /// <summary>
    /// Creates an instance of the MakeFilterChannel class with the client it is part of.
    /// </summary>
    /// <param name="client">The client to which the command will be added.</param>
    public MakeFilterChannel(DiscordSocketClient client)
    {
        _client = client;
    }

    public MakeFilterChannel(DiscordSocketClient client, Logger commandLogger) : this(client)
    {
        CommandLogger = commandLogger;
    }

    public string CommandName { get; } = "make_filter_channel";

    /// <summary>
    /// Adds the command to the client.
    /// </summary>
    /// <param name="guildID"></param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
            
        commandBuilder.WithName(CommandName);
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Creates a channel that is used to filter out YouTube shorts.");
        commandBuilder.AddOption(optionChannelName, 
            ApplicationCommandOptionType.String, 
            $"Name of the filter channel. Default: {defaultChannelName}", 
            isRequired: false);
        commandBuilder.AddOption(optionChannelCategoryName,
            ApplicationCommandOptionType.String,
            $"Channel category to add the filter to. Default: {defaultChannelCategoryName}",
            isRequired: false);
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

    /// <summary>
    /// Used to to handle the MakeFilterCommand when the SlashCommandExecuted event takes place.
    /// </summary>
    /// <param name="command">The command that has been run.</param>
    /// <returns>An asynchronous task representing a command that has been run.</returns>
    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        // Only allow the make_filter_channel command to run.
        if (command.CommandName != CommandName)
        {
            return;
        }
            
        if (command.GuildId == null)
        {
            await command.RespondAsync("This command must be used in a server!");
        }

        SocketGuildChannel channel = (SocketGuildChannel)command.Channel;
        SocketGuild guild = channel.Guild;

        IReadOnlyCollection<SocketSlashCommandDataOption> optionsReadOnly = command.Data.Options;
        Dictionary<string, string?> optionsMap = new Dictionary<string, string?>();

        foreach (SocketSlashCommandDataOption option in optionsReadOnly)
        {
            optionsMap.Add(option.Name, option.Value.ToString());
        }

        if (optionsMap.ContainsKey(optionChannelName) && optionsMap.ContainsKey(optionChannelCategoryName))
        {
            ulong filterCategory = await CreateFilterCategoryAsync(guild, optionsMap[optionChannelCategoryName]);
            ulong filterChannel = await CreateFilterChannelAsync(guild, optionsMap[optionChannelName]);
            await AssociateChannelAsync(guild, filterCategory, filterChannel);
            CommandLogger.Info($"Created filter channel. " +
                               $"Filter Channel Name: {optionsMap[optionChannelName]} " +
                               $"Filter Channel ID: {filterChannel} " +
                               $"Filter Category Name: {optionsMap[optionChannelCategoryName]} " +
                               $"Filter Category ID: {filterCategory} " +
                               $"Guild Name: {guild.Name} " +
                               $"Guild ID: {guild.Id}");
            await command.RespondAsync("Created filter channel!", ephemeral: true);
        }
        else if (optionsMap.ContainsKey(optionChannelName))
        {
            ulong filterCategory = await CreateFilterCategoryAsync(guild, defaultChannelCategoryName);
            ulong filterChannel = await CreateFilterChannelAsync(guild, optionsMap[optionChannelName]);
            await AssociateChannelAsync(guild, filterCategory, filterChannel);
            CommandLogger.Info($"Created filter channel. " +
                               $"Filter Channel Name: {optionsMap[optionChannelName]} " +
                               $"Filter Channel ID: {filterChannel} " +
                               $"Filter Category Name: {defaultChannelCategoryName} " +
                               $"Filter Category ID: {filterCategory} " +
                               $"Guild Name: {guild.Name} " +
                               $"Guild ID: {guild.Id}");
            await command.RespondAsync("Created filter channel!", ephemeral: true);

        }
        else if (optionsMap.ContainsKey(optionChannelCategoryName))
        {
            ulong filterCategory = await CreateFilterCategoryAsync(guild, optionsMap[optionChannelCategoryName]);
            ulong filterChannel = await CreateFilterChannelAsync(guild, defaultChannelName);
            await AssociateChannelAsync(guild, filterCategory, filterChannel);
            CommandLogger.Info($"Created filter channel. " +
                               $"Filter Channel Name: {defaultChannelName} " +
                               $"Filter Channel ID: {filterChannel} " +
                               $"Filter Category Name: {optionsMap[optionChannelCategoryName]} " +
                               $"Filter Category ID: {filterCategory} " +
                               $"Guild Name: {guild.Name} " +
                               $"Guild ID: {guild.Id}");
            await command.RespondAsync("Created filter channel!", ephemeral: true);
        }
        else
        {
            ulong filterCategory = await CreateFilterCategoryAsync(guild, defaultChannelCategoryName);
            ulong filterChannel = await CreateFilterChannelAsync(guild, defaultChannelName);
            await AssociateChannelAsync(guild, filterCategory, filterChannel);
            CommandLogger.Info($"Created filter channel. " +
                               $"Filter Channel Name: {defaultChannelName} " +
                               $"Filter Channel ID: {filterChannel} " +
                               $"Filter Category Name: {defaultChannelCategoryName} " +
                               $"Filter Category ID: {filterCategory} " +
                               $"Guild Name: {guild.Name} " +
                               $"Guild ID: {guild.Id}");
            await command.RespondAsync("Created filter channel!", ephemeral: true);
        }
    }

    /// <summary>
    /// Creates a new category in the a guild.
    /// </summary>
    /// <param name="guild">The guild to add the category to.</param>
    /// <param name="categoryName">What to name the category.</param>
    /// <returns>
    /// Returns an asynchronous task that represents the creation of a category.
    /// The result task will return the newly created categories ID, or if a category
    /// with the same name already exists, it returns the ID of the existing category.
    /// </returns>
    private async Task<ulong> CreateFilterCategoryAsync(SocketGuild guild, string categoryName)
    {
        IReadOnlyCollection<SocketCategoryChannel> currentCategories = guild.CategoryChannels;
        foreach (SocketCategoryChannel categoryChannel in currentCategories)
        {
            if (categoryChannel.Name == categoryName)
            {
                CommandLogger.Info($"Category {categoryName} already exists, skipping creation...");
                return categoryChannel.Id;
            }
        }

        RestCategoryChannel newCategory = await guild.CreateCategoryChannelAsync(categoryName);
        CommandLogger.Info($"Created category {newCategory.Name} || " +
                           $"Category ID: {newCategory.Id} || " +
                           $"Server Name: {guild.Name} || " +
                           $"Server ID: {guild.Id}.");
        return newCategory.Id;
    }

    /// <summary>
    /// Creates a new channel for filtering command.
    /// </summary>
    /// <param name="guild">The guild to make the channel in.</param>
    /// <param name="channelName">The name of the channel.</param>
    /// <returns>
    /// An asynchronous task that represents the creation of a channel. 
    /// The task result contains the new channel id.
    /// If the channel with the same name already exists, then it returns a task with
    /// the result of the existing channel ID.
    /// </returns>
    private async Task<ulong> CreateFilterChannelAsync(SocketGuild guild, string channelName)
    {
        ValidatedChannelName validatedChannelName = new ValidatedChannelName(channelName);
            
        foreach(SocketGuildChannel guildChannel in guild.Channels)
        {
            if (guildChannel.Name == validatedChannelName.ValidName) 
            {
                CommandLogger.Info($"Category {validatedChannelName.ValidName} already exists, skipping creation...");
                return guildChannel.Id;
            }
        }

        ITextChannel channel = await guild.CreateTextChannelAsync(validatedChannelName.ValidName);
        CommandLogger.Info($"Created channel: {validatedChannelName.ValidName} || " +
                           $"Channel ID: {channel.Id} || " +
                           $"Server Name: {guild.Name} || " +
                           $"Server ID: {guild.Id}.");
        return channel.Id;
    }

    /// <summary>
    /// Associates a channel to a category.
    /// </summary>
    /// <param name="guild">The guild that the channel and category exist in.</param>
    /// <param name="category">The id of the category to add the channel to.</param>
    /// <param name="channel">The channel to add to the category.</param>
    /// <returns>
    /// An asynchronous task that represents the association of a channel and category.
    /// </returns>
    private async Task AssociateChannelAsync(SocketGuild guild, ulong category, ulong channel)
    {
        RestGuild updatedGuild = await _client.Rest.GetGuildAsync(guild.Id);
        RestGuildChannel guildChannel = await updatedGuild.GetTextChannelAsync(channel);
        RestTextChannel socketGuildChannel = await updatedGuild.GetTextChannelAsync(channel);

        EmbedFieldBuilder channelIdFieldBuilder = new EmbedFieldBuilder();
        channelIdFieldBuilder.WithName("Channel ID:");
        channelIdFieldBuilder.WithValue(channel.ToString());
        channelIdFieldBuilder.WithIsInline(true);
            
        EmbedFieldBuilder detailsFieldBuilder = new EmbedFieldBuilder();
        string detailsMessage = "You can now use it for bots or services that auto post to a channel. " +
                                "\n Use the channel ID found above when setting those services." +
                                "\n Also please be sure to Assosiate any channel where you want the videos" +
                                "poseted to.";
        detailsFieldBuilder.WithName("How this is used:");
        detailsFieldBuilder.WithValue(detailsMessage);
        detailsFieldBuilder.WithIsInline(false);
            
        EmbedBuilder embed = new EmbedBuilder();
        embed.WithTitle("Filter Channel Details");
        embed.WithDescription("You have made this a channel a filter channel!");
        embed.WithColor(Color.Green);
        embed.AddField(channelIdFieldBuilder);
        embed.AddField(detailsFieldBuilder);
            
        // Modify the channel to be in the right category.
        await guildChannel.ModifyAsync(prop => prop.CategoryId = category);

        CommandLogger.Info($"Added channel to category. " +
                           $"Channel ID: {channel} || " +
                           $"Category ID: {category} || " +
                           $"Server Name: {guild.Name}.");
            
        await socketGuildChannel.SendMessageAsync(embed: embed.Build());
    }
}