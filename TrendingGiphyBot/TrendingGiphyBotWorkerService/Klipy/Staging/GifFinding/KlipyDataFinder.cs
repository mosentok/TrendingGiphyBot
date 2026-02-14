using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyDataFinder
(
    IKlipyTrendingGifFinder _giphyTrendingGifFinder,
    IKlipySearchGifFinder _giphySearchGifFinder,
    IKlipyRandomGifFinder _giphyRandomGifFinder
) : IKlipyDataFinder
{
    public KlipyDataWithSource? TryGetUnseenGif(ChannelSettingsModel channel)
    {
        var trendingResult = _giphyTrendingGifFinder.TryGetTrendingGif(channel);

        if (trendingResult is not null)
            return new KlipyDataWithSource(trendingResult, KlipySourceType.Trending);

        var searchResult = _giphySearchGifFinder.TryGetSearchGif(channel);

        if (searchResult is not null)
            return new KlipyDataWithSource(searchResult, KlipySourceType.Search);

        var randomResult = _giphyRandomGifFinder.TryGetRandomGif(channel);

        if (randomResult is not null)
            return new KlipyDataWithSource(randomResult, KlipySourceType.Random);

        return null;
    }
}

