using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipySearchGifFinder
(
    IKlipySearchCache _giphySearchCache,
    IOptions<AppConfig> _appConfig
) : IKlipySearchGifFinder
{
    public Maybe<KlipyData> TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Giphy.Staging.EnableSearchGifs || channel.GifKeyword is null or "")
            return new();

        if (channel.GiphyPosts.Count == 0)
        {
            var firstGif = _giphySearchCache.GetFirstGif(channel.GifKeyword);

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenGiphyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var keywordGif = _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenGiphyDataIds);

        return keywordGif is not null
            ? new(keywordGif)
            : new();
    }
}
