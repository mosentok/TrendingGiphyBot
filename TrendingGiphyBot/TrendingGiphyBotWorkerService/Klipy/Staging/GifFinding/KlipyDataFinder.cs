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
    public KlipyData? TryGetUnseenGif(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        var trendingResult = _giphyTrendingGifFinder.TryGetTrendingGif(channel);

        if (trendingResult is not null)
            return trendingResult;

        var searchResult = _giphySearchGifFinder.TryGetSearchGif(channel);

        if (searchResult is not null)
            return searchResult;

        var randomResult = _giphyRandomGifFinder.TryGetRandomGif(channel, cancellationToken);

        if (randomResult is not null)
            return randomResult;

        return null;
    }
}
