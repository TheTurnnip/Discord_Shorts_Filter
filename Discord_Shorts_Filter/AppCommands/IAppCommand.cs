using Discord.WebSocket;

namespace Discord_Shorts_Filter.AppCommands;

public interface IAppCommand
{
    protected DiscordSocketClient Client { get; set; }
    
    public Task AddCommandAsync();
    
    public Task HandleCommandAsync(SocketSlashCommand command);
}