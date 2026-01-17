using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsModel
{
    public ulong ChannelId { get; set; }
    public int Frequency { get; set; }
    public int IntervalId { get; set; }
    public string? HowOften { get; set; }
    public string? GifPostingBehavior { get; set; }
    public string? GifKeyword { get; set; }
    public string? PostingHoursFrom { get; set; }
    public string? PostingHoursTo { get; set; }
    public string? TimeZone { get; set; }
    public virtual Interval Interval { get; set; } = null!;
    public virtual ICollection<GifPost>? GifPosts { get; set; }
}
