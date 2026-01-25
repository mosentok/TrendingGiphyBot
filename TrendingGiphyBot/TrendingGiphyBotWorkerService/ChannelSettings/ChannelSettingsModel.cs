using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsModel
{
    public ulong ChannelId { get; set; }
    public int Frequency { get; set; }
    public int IntervalId { get; set; }
    public int GifPostingBehaviorId { get; set; }
    public string? GifKeyword { get; set; }
    public int? PostingHoursFrom { get; set; }
    public int? PostingHoursTo { get; set; }
    public string? UtcOffset { get; set; }
    public virtual Interval Interval { get; set; } = null!;
    public virtual GifPostingBehaviorModel GifPostingBehavior { get; set; } = null!;
    public virtual ICollection<GifPost>? GifPosts { get; set; }
}
