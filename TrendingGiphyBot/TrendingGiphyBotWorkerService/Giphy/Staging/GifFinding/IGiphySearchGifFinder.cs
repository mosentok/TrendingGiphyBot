using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphySearchGifFinder
{
    Maybe<GiphyData> TryGetSearchGif(ChannelSettingsModel channel);
}