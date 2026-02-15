using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyDataFinder
(
    IGiphyTrendingGifFinder _giphyTrendingGifFinder,
    IGiphySearchGifFinder _giphySearchGifFinder,
    IGiphyRandomGifFinder _giphyRandomGifFinder
) : IGiphyDataFinder
{
    public GiphyDataWithSource? TryGetUnseenGif(ChannelSettingsModel channel)
    {
        var trendingResult = _giphyTrendingGifFinder.TryGetTrendingGif(channel);

        if (trendingResult is not null)
            return new GiphyDataWithSource(trendingResult, GiphySourceType.Trending);

        var searchResult = _giphySearchGifFinder.TryGetSearchGif(channel);

        if (searchResult is not null)
            return new GiphyDataWithSource(searchResult, GiphySourceType.Search);

        var randomResult = _giphyRandomGifFinder.TryGetRandomGif(channel);

        if (randomResult is not null)
            return new GiphyDataWithSource(randomResult, GiphySourceType.Random);

        return null;
    }
}
