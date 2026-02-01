using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphySearchGifFinder
{
    Maybe<GiphyData> TryGetSearchGif(ChannelSettingsModel channel);
}