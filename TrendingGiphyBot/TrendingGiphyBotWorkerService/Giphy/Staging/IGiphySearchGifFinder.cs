using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGiphySearchGifFinder
{
    Maybe<GiphyData> TryGetSearchGif(ChannelSettingsModel channel);
}