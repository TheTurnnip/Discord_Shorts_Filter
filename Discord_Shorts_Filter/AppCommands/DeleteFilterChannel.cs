using Discord_Shorts_Filter.Logging;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Discord_Shorts_Filter.AppCommands;

public class DeleteFilterChannel : IAppCommand
{
    private DiscordSocketClient _client;

    public DeleteFilterChannel(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public async Task AddCommandAsync()
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();

        commandBuilder.WithName("delete_filter_channel");
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Delete a channel that is being used to filter out shorts.");
        commandBuilder.AddOption("filter_channel_id", 
                                ApplicationCommandOptionType.Integer, 
                                "Deletes a channel and remvoes it from the filter system.",
                                true);
        try
        {
            await _client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
        }
        catch (HttpException exception) 
        {
            string response = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Logger.Error(response);
        }
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        if (command.GuildId == null)
        {
            await command.RespondAsync("This command must be used in a server!");
        }

        // Get the channel ID of the channel that needs to be deleted.
        int deleteChannelId = (int)command.Data.Options.FirstOrDefault(
            option => option.Name == "filter_channel_id"
            )!.Value;
        
        Logger.Debug(deleteChannelId.ToString());
    }
}