using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Trending;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyTrendingGifFinder
(
    IGiphyTrendingCache _giphyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyTrendingGifFinder
{
    public GiphyData? TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableTrendingGifs)
            return null;

        var rating = channel.GiphyRating ?? "all";

        var retentionDays = channel.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        if (channel.GiphyPosts is null or { Count: 0 })
            return _giphyTrendingCache.GetFirstGif(rating);

        var seenGiphyDataIds = channel.GiphyPosts
            .Where(s => s.CreatedUtc >= cutoffDate)
            .Select(s => s.GiphyDataId)
            .ToArray();

        return _giphyTrendingCache.GetFirstUnseenGif(seenGiphyDataIds, rating);
    }
}
