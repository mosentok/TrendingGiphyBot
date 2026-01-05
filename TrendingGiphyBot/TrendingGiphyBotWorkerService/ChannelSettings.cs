namespace TrendingGiphyBotWorkerService;

public class ChannelSettings
{
    public ulong ChannelId { get; set; }
    public string? HowOften { get; set; }
    public string? GifPostingBehavior { get; set; }
    public string? GifKeyword { get; set; }
    public string? PostingHoursFrom { get; set; }
    public string? PostingHoursTo { get; set; }
    public string? TimeZone { get; set; }
}
