using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphyTrendingGifFinder
{
    Maybe<GiphyData> TryGetTrendingGif(ChannelSettingsModel channel);
}