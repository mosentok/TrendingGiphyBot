using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsModel
{
    public ulong ChannelId { get; set; }
    public required int Frequency { get; set; }
    public required Interval Interval { get; set; }
    // TODO setter should throw an exception if value is None
    public required GifPostingBehaviorKind GifPostingBehavior { get; set; }
    public required string GiphyRating { get; set; }
    public required GifSourceKind GifSource { get; set; }
    public string? GifKeyword { get; set; }
    public PostingHours? PostingHours { get; set; }
    public int? RetentionDays { get; set; }
    public virtual ICollection<GiphyPost> GiphyPosts { get; set; } = [];
    public virtual ICollection<KlipyPost> KlipyPosts { get; set; } = [];
}
