using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord_Shorts_Filter.Logging;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands
{
    internal static class MakeFilterChannel
    {
        public static async Task AddCommand(DiscordSocketClient client)
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            
            command.WithName("make_filter_channel");
            command.WithDescription("Creates a channel that is used to filter out YouTube shorts.");
            command.AddOption("channel_name", 
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
            await command.RespondAsync(text: "The filter channel has been created.", ephemeral: true);
        }
    }
}
