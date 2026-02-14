using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

public record GiphySourceSelection(
    GiphyData Data,
    GiphySourceType SourceType
);
