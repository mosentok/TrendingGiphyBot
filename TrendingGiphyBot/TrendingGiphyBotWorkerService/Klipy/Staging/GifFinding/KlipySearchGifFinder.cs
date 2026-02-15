using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Search;

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
            return _giphySearchCache.GetFirstGif(channel.GifKeyword);

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();

        return _giphySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenKlipyDataIds);
    }
}
