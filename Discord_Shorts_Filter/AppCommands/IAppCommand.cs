using Discord.WebSocket;

namespace Discord_Shorts_Filter.AppCommands;

public interface IAppCommand
{
    public Task AddCommandAsync(ulong guildID);
    
    public Task HandleCommandAsync(SocketSlashCommand command);
}