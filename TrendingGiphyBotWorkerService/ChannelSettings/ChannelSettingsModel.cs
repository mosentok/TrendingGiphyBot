using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsModel
{
    public ulong ChannelId { get; set; }
    public int Frequency { get; set; }
    public GifPostingBehaviorKind GifPostingBehavior { get; set; }
    public string? GifKeyword { get; set; }
    public string? GiphyRating { get; set; }
    public GifSourceKind? GifSource { get; set; }
    public IntervalDescription Interval { get; set; }
    public int? PostingHoursFrom { get; set; }
    public int? PostingHoursTo { get; set; }
    public int? RetentionDays { get; set; }
    public string? UtcOffset { get; set; }
    public virtual ICollection<GiphyPost> GiphyPosts { get; set; } = [];
    public virtual ICollection<KlipyPost> KlipyPosts { get; set; } = [];
}
