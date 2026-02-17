using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public record ChannelSettingsDto(
    ulong ChannelId,
    int Frequency,
    GifPostingBehaviorKind GifPostingBehavior,
    string GifPostingBehaviorDescription,
    string? GifKeyword,
    string? GiphyRating,
    GifSourceKind? GifSource,
    IntervalDescription Interval,
    int? PostingHoursFrom,
    int? PostingHoursTo,
    int? RetentionDays,
    string? UtcOffset
);
