using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public record ChannelSettingsDto(
    ulong ChannelId,
    int Frequency,
    // TODO setter should throw an exception if value is None
    GifPostingBehaviorKind GifPostingBehavior,
    string GifPostingBehaviorDescription,
    string? GifKeyword,
    string? GiphyRating,
    GifSourceKind? GifSource,
    Interval Interval,
    PostingHours? PostingHours,
    int? RetentionDays
);
