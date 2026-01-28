using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifFinder(
    IGiphyClient _giphyClient,
    IGiphySearchCache _giphySearchCache,
    IGiphyTrendingCache _giphyTrendingCache,
    GifPostStageConfig _gifPostStageConfig
) : IGifFinder
{
    public async Task<Maybe<GiphyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
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

        if (channel.GifKeyword is not null or "")
        {
            var keywordGif = _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenGiphyDataIds);

            return keywordGif is not null
                ? new(keywordGif)
                : new();
        }

        var attempts = 0;

        do
        {
            var randomGif = await _giphyClient.GetRandomGifAsync(cancellationToken: cancellationToken);

            var randomGifHasAlreadyBeenSeen = seenGiphyDataIds.Contains(randomGif.Data.Id);

            if (!randomGifHasAlreadyBeenSeen)
                return new(randomGif.Data);

            attempts++;
        } while (attempts < _gifPostStageConfig.MaxRandomGifAttempts);

        return new();
    }
}
