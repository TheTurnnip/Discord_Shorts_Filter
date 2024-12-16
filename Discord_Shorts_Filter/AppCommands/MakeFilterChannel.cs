using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Newtonsoft.Json;

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
                try
                {
                    ulong filterCategory = await CreateFilterCategory(guild, optionsMap[addChannelCategoryName]);
                    ulong filterChannel = await CreateFilterChannel(guild, optionsMap[addChannelName]);
                    if (filterCategory == 0)
                    {
                        Logger.Debug("The category already exists.");
                    }
                    await AssociateChannel(guild, filterCategory, filterChannel);
                }
                catch (Exception exception)
                {
                    Logger.Error($"There was an error encountered associating channel to category: {exception}");
                }

            }
            else if (optionsMap.ContainsKey(addChannelName))
            {
                
            }
            else if (optionsMap.ContainsKey(addChannelCategoryName))
            {

            }
            else
            {

            }

            await command.RespondAsync("Created filter channel!", ephemeral: true);
        }

        /// <summary>
        /// Creates a new category in the a guild.
        /// </summary>
        /// <param name="guild">The guild to add the category to.</param>
        /// <param name="categoryName">What to name the category.</param>
        /// <returns>
        /// Returns the id of the category that has been created. If it already exists,
        /// the existing category id is returned.
        /// </returns>
        private async Task<ulong> CreateFilterCategory(SocketGuild guild, string categoryName)
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
        /// 
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        private async Task<ulong> CreateFilterChannel(SocketGuild guild, string channelName)
        {
            foreach(SocketGuildChannel guildChannel in guild.Channels)
            {
                if (guildChannel.Name == channelName) 
                {
                    return 0;
                }
            }

            ITextChannel channel = await guild.CreateTextChannelAsync(channelName);
            Logger.Info($"Created channel: {channelName} In Server: {guild.Name}");
            return channel.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="category"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private async Task AssociateChannel(SocketGuild guild, ulong category, ulong channel)
        {
            RestGuild updatedGuild = await client.Rest.GetGuildAsync(guild.Id);
            RestGuildChannel guildChannel = await updatedGuild.GetTextChannelAsync(channel);

            await guildChannel.ModifyAsync(prop => prop.CategoryId = category);

            Logger.Info($"Added the channel to the category.");
        }
    }
}
