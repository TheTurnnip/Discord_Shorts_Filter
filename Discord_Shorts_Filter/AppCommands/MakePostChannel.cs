using Discord;
using Discord.WebSocket;

namespace Discord_Shorts_Filter.AppCommands;

public class MakePostChannel : IAppCommand
{
    private DiscordSocketClient _client;

    public MakePostChannel(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public async Task AddCommandAsync()
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
        
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        throw new NotImplementedException();
    }
}