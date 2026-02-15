using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Trending;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyTrendingGifFinder
(
    IKlipyTrendingCache _giphyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipyTrendingGifFinder
{
    public KlipyData? TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableTrendingGifs)
            return null;

        var retentionDays = channel.RetentionDays ?? _appConfig.CurrentValue.RetentionDays.DefaultDays;
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        if (channel.KlipyPosts.Count == 0)
            return _giphyTrendingCache.GetFirstGif();

        var seenKlipyDataIds = channel.KlipyPosts
            .Where(s => s.CreatedUtc >= cutoffDate)
            .Select(s => s.KlipyDataId)
            .ToArray();

        return _giphyTrendingCache.GetFirstUnseenGif(seenKlipyDataIds);
    }
}
