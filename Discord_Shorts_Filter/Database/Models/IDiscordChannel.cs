using Discord.WebSocket;
namespace Discord_Shorts_Filter.Database.Models;
using Discord;


public interface IDiscordChannel
{
    public ulong ChannelId { get; set; }
    public ulong GuildId { get; set; }
    public string ChannelName { get; set; }

    public SocketTextChannel ToSocketTextChannel(DiscordSocketClient client);
}