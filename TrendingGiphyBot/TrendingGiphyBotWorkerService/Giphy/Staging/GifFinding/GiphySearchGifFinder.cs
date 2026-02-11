using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphySearchGifFinder
(
    IGiphySearchCache _giphySearchCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphySearchGifFinder
{
    public GiphyData? TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableSearchGifs || channel.GifKeyword is null or "")
            return null;

        if (channel.GiphyPosts is null or { Count: 0 })
        {
            var firstGif = _giphySearchCache.GetFirstGif(channel.GifKeyword);

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var keywordGif = _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenGiphyDataIds);

        return keywordGif is not null
            ? keywordGif
            : null;
    }
}
