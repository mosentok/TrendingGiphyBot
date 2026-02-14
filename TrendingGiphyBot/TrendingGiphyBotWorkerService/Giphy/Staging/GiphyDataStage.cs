using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

[RegisterSingleton]
public class GiphyDataStage(
    ILogger<GiphyDataStage> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGiphyDataFinder _giphyDataFinder
) : IGiphyDataStage
{
    readonly Dictionary<ulong, GiphyData> _trendingItems = [];
    readonly Dictionary<ulong, GiphyData> _searchItems = [];
    readonly Dictionary<ulong, GiphyData> _randomItems = [];

    public IImmutableDictionary<ulong, GiphyData> GetTrendingGiphyPostStage() => _trendingItems.ToImmutableDictionary();

    public IImmutableDictionary<ulong, GiphyData> GetSearchGiphyPostStage() => _searchItems.ToImmutableDictionary();

    public IImmutableDictionary<ulong, GiphyData> GetRandomGiphyPostStage() => _randomItems.ToImmutableDictionary();

    public void EvictTrending(ulong channelId) => _trendingItems.Remove(channelId);

    public void EvictSearch(ulong channelId) => _searchItems.Remove(channelId);

    public void EvictRandom(ulong channelId) => _randomItems.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.GiphyPosts)
            .Where(s =>
                !_trendingItems.Keys.Contains(s.ChannelId) &&
                !_searchItems.Keys.Contains(s.ChannelId) &&
                !_randomItems.Keys.Contains(s.ChannelId) &&
                s.Frequency > 0)
            .ToAsyncEnumerable()
            .WithCancellation(cancellationToken);

        await foreach (var channel in activeChannels)
        {
            var unseenGifWithSource = _giphyDataFinder.TryGetUnseenGif(channel);

            if (unseenGifWithSource is null)
                continue;

            var dictionary = unseenGifWithSource.SourceType switch
            {
                GiphySourceType.Trending => _trendingItems,
                GiphySourceType.Search => _searchItems,
                GiphySourceType.Random => _randomItems,
                _ => _randomItems
            };

            dictionary[channel.ChannelId] = unseenGifWithSource.Data;
        }

        _logger.LogGiphyStageCount(_trendingItems.Count + _searchItems.Count + _randomItems.Count);
    }
}

