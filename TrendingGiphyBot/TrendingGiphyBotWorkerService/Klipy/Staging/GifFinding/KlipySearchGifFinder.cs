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
    IKlipySearchCache _klipySearchCache,
    IOptions<AppConfig> _appConfig
) : IKlipySearchGifFinder
{
    public Maybe<KlipyData> TryGetSearchGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Klipy.Staging.EnableSearchGifs || channel.GifKeyword is null or "")
            return new();

        if (channel.KlipyPosts.Count == 0)
        {
            var firstKlipy = _klipySearchCache.GetFirstGif(channel.GifKeyword);

            return firstKlipy is not null
                ? new(firstKlipy)
                : new();
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var keywordKlipy = _klipySearchCache.GetFirstUnseenGif(channel.GifKeyword, seenKlipyDataIds);

        return keywordKlipy is not null
            ? new(keywordKlipy)
            : new();
    }
}
