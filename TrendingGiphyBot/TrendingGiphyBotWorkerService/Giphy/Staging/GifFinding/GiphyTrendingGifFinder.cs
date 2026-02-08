using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
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

        if (channel.GiphyPosts is null or { Count: 0 })
        {
            var firstGif = _giphyTrendingCache.GetFirstGif();

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var firstUnseenGif = _giphyTrendingCache.GetFirstUnseenGif(seenGiphyDataIds);

        if (firstUnseenGif is not null)
            return new(firstUnseenGif);

        return new();
    }
}
