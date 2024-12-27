namespace Discord_Shorts_Filter.Database.Models;

public class DiscordFilterChannels : IDiscordChannel
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public string ChannelName { get; set; }
}