using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifFinder(
    IGiphyTrendingGifFinder _giphyTrendingGifFinder,
    IGiphySearchGifFinder _giphySearchGifFinder,
    IGiphyRandomGifFinder _giphyRandomGifFinder
) : IGifFinder
{
    public async Task<Maybe<GiphyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
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
