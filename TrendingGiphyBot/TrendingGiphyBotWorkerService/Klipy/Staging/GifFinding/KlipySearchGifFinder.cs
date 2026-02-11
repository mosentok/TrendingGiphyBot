using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipySearchGifFinder
(
    IKlipySearchCache _giphySearchCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipySearchGifFinder
{
    public KlipyData? TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableSearchGifs || channel.GifKeyword is null or "")
            return null;

        if (channel.KlipyPosts.Count == 0)
        {
            var firstGif = _giphySearchCache.GetFirstGif(channel.GifKeyword);

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var keywordGif = _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenKlipyDataIds);

        return keywordGif is not null
            ? keywordGif
            : null;
    }
}
