using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GiphyTrendingGifFinder
(
    IGiphyTrendingCache _giphyTrendingCache,
    IOptions<AppConfig> _appConfig
) : IGiphyTrendingGifFinder
{
    public Maybe<GiphyData> TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Giphy.Staging.EnableTrendingGifs)
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
