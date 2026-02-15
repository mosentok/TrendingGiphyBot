using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Random;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyRandomGifFinder
(
    IGiphyRandomCache _giphyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyRandomGifFinder
{
    public GiphyData? TryGetRandomGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableRandomGifs)
            return null;

        var rating = channel.GiphyRating ?? "all";

        var retentionDays = channel.RetentionDays ?? _appConfig.CurrentValue.RetentionDays.DefaultDays;
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        if (channel.GiphyPosts is null or { Count: 0 })
            return _giphyRandomCache.GetFirstGif(rating);

        var seenGiphyDataIds = channel.GiphyPosts
            .Where(s => s.CreatedUtc >= cutoffDate)
            .Select(s => s.GiphyDataId)
            .ToArray();

        return _giphyRandomCache.GetFirstUnseenGif(seenGiphyDataIds, rating);
    }
}
