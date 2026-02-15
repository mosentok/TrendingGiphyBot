using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public record GiphyGifPostSelection(GiphyData Data, GiphySourceType SourceType);
