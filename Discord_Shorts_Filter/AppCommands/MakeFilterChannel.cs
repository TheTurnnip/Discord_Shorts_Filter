using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Discord_Shorts_Filter.AppCommands
{
    /// <summary>
    /// A class that represents the make_filter_channel command, that is used when filtering videos.
    /// </summary>
    internal class MakeFilterChannel
    {

        private DiscordSocketClient client;
        private static readonly string addChannelName = "channel_name";
        private static readonly string defaultChannelName = "filter_channel";
        private static readonly string addChannelCategoryName = "category_name";
        private static readonly string defaultChannelCategoryName = "Filters";

        /// <summary>
        /// Creates an instance of the MakeFilterChannel class with the client it is part of.
        /// </summary>
        /// <param name="client">The client to which the command will be added.</param>
        internal MakeFilterChannel(DiscordSocketClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Adds the command to the client.
        /// </summary>
        /// <returns>A task that represents an asynchronous operation.</returns>
        public async Task AddCommandAsync()
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            
            command.WithName("make_filter_channel");
            command.WithDefaultMemberPermissions(GuildPermission.Administrator);
            command.WithDescription("Creates a channel that is used to filter out YouTube shorts.");
            command.AddOption(addChannelName, 
                              ApplicationCommandOptionType.String, 
                              $"Name of the filter channel. Default: {defaultChannelName}", 
                              isRequired: false);
            command.AddOption(addChannelCategoryName,
                              ApplicationCommandOptionType.String,
                              $"Channel category to add the filter to. Default: {defaultChannelCategoryName}",
                              isRequired: false);
            try
            {
                await client.CreateGlobalApplicationCommandAsync(command.Build());
            }
            catch (HttpException exception) 
            {
                string response = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Logger.Error(response);
            }
        }

        /// <summary>
        /// Used to to handle the MakeFilterCommand when the SlashCommandExecuted event takes place.
        /// </summary>
        /// <param name="command">The command that has been run.</param>
        /// <returns>An asynchronous task representing a command that has been run.</returns>
        public async Task CommandHandler(SocketSlashCommand command)
        {
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

            if (optionsMap.ContainsKey(addChannelName) && optionsMap.ContainsKey(addChannelCategoryName))
            {
                ulong filterCategory = await CreateFilterCategoryAsync(guild, optionsMap[addChannelCategoryName]);
                ulong filterChannel = await CreateFilterChannelAsync(guild, optionsMap[addChannelName]);
                await AssociateChannelAsync(guild, filterCategory, filterChannel);
            }
            else if (optionsMap.ContainsKey(addChannelName))
            {
                ulong filterCategory = await CreateFilterCategoryAsync(guild, defaultChannelCategoryName);
                ulong filterChannel = await CreateFilterChannelAsync(guild, optionsMap[addChannelName]);
                await AssociateChannelAsync(guild, filterCategory, filterChannel);
            }
            else if (optionsMap.ContainsKey(addChannelCategoryName))
            {
                ulong filterCategory = await CreateFilterCategoryAsync(guild, optionsMap[addChannelCategoryName]);
                ulong filterChannel = await CreateFilterChannelAsync(guild, defaultChannelName);
                await AssociateChannelAsync(guild, filterCategory, filterChannel);
            }
            else
            {
                ulong filterCategory = await CreateFilterCategoryAsync(guild, defaultChannelCategoryName);
                ulong filterChannel = await CreateFilterChannelAsync(guild, defaultChannelName);
                await AssociateChannelAsync(guild, filterCategory, filterChannel);
            }

            await command.RespondAsync("Created filter channel!", ephemeral: true);
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
                    Logger.Info($"Category {categoryName} already exists, skipping creation...");
                    return categoryChannel.Id;
                }
            }

            RestCategoryChannel newCategory = await guild.CreateCategoryChannelAsync(categoryName);
            Logger.Info($"Created category {newCategory.Name} || ID = {newCategory.Id}");
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
            foreach(SocketGuildChannel guildChannel in guild.Channels)
            {
                if (guildChannel.Name == channelName) 
                {
                    return guildChannel.Id;
                }
            }

            ITextChannel channel = await guild.CreateTextChannelAsync(channelName);
            Logger.Info($"Created channel: {channelName} In Server: {guild.Name}");
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
            RestGuild updatedGuild = await client.Rest.GetGuildAsync(guild.Id);
            RestGuildChannel guildChannel = await updatedGuild.GetTextChannelAsync(channel);
            SocketTextChannel socketGuildChannel = (SocketTextChannel)client.GetGuild(guild.Id).GetChannel(channel);

            // Modify the channel to be in the right category.
            await guildChannel.ModifyAsync(prop => prop.CategoryId = category);

            Logger.Info($"Added the channel to the category.");

            string message = $"This channel has been set as a YouTube shorts filter. \n" +
                             $"Your Channel ID is: `{channel}` \n" +
                             $"Please use when setting up automatic video posting (ie. MEE6).";
            await socketGuildChannel.SendMessageAsync(message);
        }
    }
}
