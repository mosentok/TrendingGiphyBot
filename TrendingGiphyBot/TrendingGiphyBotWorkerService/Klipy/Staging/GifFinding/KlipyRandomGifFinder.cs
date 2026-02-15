using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Random;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyRandomGifFinder
(
    IKlipyRandomCache _klipyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipyRandomGifFinder
{
    public KlipyData? TryGetRandomGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableRandomGifs)
            return null;

        var retentionDays = channel.RetentionDays ?? _appConfig.CurrentValue.RetentionDays.DefaultDays;
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        if (channel.KlipyPosts.Count == 0)
            return _klipyRandomCache.GetFirstGif();

        var seenKlipyDataIds = channel.KlipyPosts
            .Where(s => s.CreatedUtc >= cutoffDate)
            .Select(s => s.KlipyDataId)
            .ToArray();

        return _klipyRandomCache.GetFirstUnseenGif(seenKlipyDataIds);
    }
}
