using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGiphyTrendingGifFinder
{
    Maybe<GiphyData> TryGetTrendingGif(ChannelSettingsModel channel);
}