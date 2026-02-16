using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public record ChannelSettingsDto(
    ulong ChannelId,
    int Frequency,
    int GifPostingBehaviorId,
    string GifPostingBehaviorDescription,
    string? GifKeyword,
    string? GiphyRating,
    GifSourceKind? GifSource,
    int IntervalId,
    int? PostingHoursFrom,
    int? PostingHoursTo,
    int? RetentionDays,
    string? UtcOffset
);
