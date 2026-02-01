using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public class GiphySearchGifFinder
(
IGiphySearchCache _giphySearchCache,
IOptions<AppConfig> _appConfig
) : IGiphySearchGifFinder
{
    public Maybe<GiphyData> TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Giphy.Staging.EnableSearchGifs || channel.GifKeyword is null or "")
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
