using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Discord_Shorts_Filter.tools;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands
{
    internal static class MakeFilterChannel
    {
 
        private static readonly string defaultChannelName = "filter_channel";
        private static readonly string addChannelOptionName = "channel_name";
        
        public static async Task AddCommand(DiscordSocketClient client)
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            
            command.WithName("make_filter_channel");
            command.WithDescription("Creates a channel that is used to filter out YouTube shorts.");
            command.AddOption(addChannelOptionName, 
                              ApplicationCommandOptionType.String, 
                              "The name of the channel that will be created and use for filtering.\nDefaults to: \"filter_channel\"", 
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

        public static async Task CommandHandler(SocketSlashCommand command)
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

            if (optionsMap.ContainsKey(addChannelOptionName))
            {
                string validChannelName = new ValidatedChannelName(optionsMap[addChannelOptionName]).ValidName;
                await CreateFilterChannel(guild, validChannelName);
            }
            else
            {
                await CreateFilterChannel(guild, defaultChannelName);
            }

            await command.RespondAsync("Created filter channel!", ephemeral: true);
        }

        private static async Task CreateFilterChannel(SocketGuild guild, string channelName)
        {
            ITextChannel channel = await guild.CreateTextChannelAsync(channelName);
            Logger.Info($"Created channel: {channelName} In Server: {guild.Name}");
        }
    }
}
