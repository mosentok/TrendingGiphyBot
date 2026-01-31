using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

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
public class GiphyRandomGifFinder
(
    IGiphyClient _giphyClient,
    GifPostStageConfig _gifPostStageConfig
) : IGiphyRandomGifFinder
{
    public async Task<Maybe<GiphyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        if (!_gifPostStageConfig.EnableRandomGifs)
            return new();

        var attempts = 0;

        do
        {
            var randomGif = await _giphyClient.GetRandomGifAsync(cancellationToken: cancellationToken);

            if (channel.GifPosts is null or { Count: 0 })
                return new(randomGif.Data);

            var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
            var randomGifHasAlreadyBeenSeen = seenGiphyDataIds.Contains(randomGif.Data.Id);

            if (!randomGifHasAlreadyBeenSeen)
                return new(randomGif.Data);

            attempts++;
        } while (attempts < _gifPostStageConfig.MaxRandomGifAttempts);

        return new();
    }
}
public class GiphySearchGifFinder
(
    IGiphySearchCache _giphySearchCache,
    GifPostStageConfig _gifPostStageConfig
) : IGiphySearchGifFinder
{
    public Maybe<GiphyData> TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_gifPostStageConfig.EnableSearchGifs || channel.GifKeyword is null or "")
            return new();

        if (channel.GifPosts is null or { Count: 0 })
        {
            var firstGif = _giphySearchCache.GetFirstGif(channel.GifKeyword);

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
        var keywordGif = _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenGiphyDataIds);

        return keywordGif is not null
            ? new(keywordGif)
            : new();
    }
}
public class GiphyTrendingGifFinder
(
    IGiphyTrendingCache _giphyTrendingCache,
    GifPostStageConfig _gifPostStageConfig
) : IGiphyTrendingGifFinder
{
    public Maybe<GiphyData> TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_gifPostStageConfig.EnableTrendingGifs)
            return new();

        if (channel.GifPosts is null or { Count: 0 })
        {
            var firstGif = _giphyTrendingCache.GetFirstGif();

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
        var firstUnseenGif = _giphyTrendingCache.GetFirstUnseenGif(seenGiphyDataIds);

        if (firstUnseenGif is not null)
            return new(firstUnseenGif);

        if (channel.GifPostingBehaviorId != GifPostingBehaviorKind.TrendingGifsWithRandomGifs.AsInt())
            return new();

        return new();
    }
}
