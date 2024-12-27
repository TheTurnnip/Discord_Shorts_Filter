namespace Discord_Shorts_Filter.Database.Models;

public class FilterPostMaps
{
    public int Id { get; set; }
    public int FilterChannelID { get; set; }
    public int PostChannelID { get; set; }
}