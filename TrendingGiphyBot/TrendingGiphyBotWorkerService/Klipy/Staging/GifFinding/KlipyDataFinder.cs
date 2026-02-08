using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyDataFinder
(
    IKlipyTrendingGifFinder _giphyTrendingGifFinder,
    IKlipySearchGifFinder _giphySearchGifFinder,
    IKlipyRandomGifFinder _giphyRandomGifFinder
) : IKlipyDataFinder
{
    public async Task<Maybe<KlipyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        var trendingResult = _giphyTrendingGifFinder.TryGetTrendingGif(channel);

        if (trendingResult.Success)
            return trendingResult;

        var searchResult = _giphySearchGifFinder.TryGetSearchGif(channel);

        if (searchResult.Success)
            return searchResult;

        var randomResult = await _giphyRandomGifFinder.TryGetRandomGifAsync(channel, cancellationToken);

        if (randomResult.Success)
            return randomResult;

        return new();
    }
}
