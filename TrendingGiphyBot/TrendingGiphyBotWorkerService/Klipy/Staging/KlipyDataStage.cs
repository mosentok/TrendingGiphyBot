using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

[RegisterSingleton]
public class KlipyDataStage(
    ILogger<KlipyDataStage> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IKlipyDataFinder _klipyDataFinder
) : IKlipyDataStage
{
    readonly Dictionary<ulong, KlipyData> _trendingItems = [];
    readonly Dictionary<ulong, KlipyData> _searchItems = [];
    readonly Dictionary<ulong, KlipyData> _randomItems = [];

    public IImmutableDictionary<ulong, KlipyData> GetTrendingKlipyPostStage() => _trendingItems.ToImmutableDictionary();

    public IImmutableDictionary<ulong, KlipyData> GetSearchKlipyPostStage() => _searchItems.ToImmutableDictionary();

    public IImmutableDictionary<ulong, KlipyData> GetRandomKlipyPostStage() => _randomItems.ToImmutableDictionary();

    public void EvictTrending(ulong channelId) => _trendingItems.Remove(channelId);

    public void EvictSearch(ulong channelId) => _searchItems.Remove(channelId);

    public void EvictRandom(ulong channelId) => _randomItems.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.KlipyPosts)
            .Where(s =>
                !_trendingItems.Keys.Contains(s.ChannelId) &&
                !_searchItems.Keys.Contains(s.ChannelId) &&
                !_randomItems.Keys.Contains(s.ChannelId) &&
                s.Frequency > 0)
            .ToAsyncEnumerable();

        await foreach (var channel in activeChannels)
        {
            var unseenGifWithSource = _klipyDataFinder.TryGetUnseenGif(channel, cancellationToken);

            if (unseenGifWithSource is null)
                continue;

            var dictionary = unseenGifWithSource.SourceType switch
            {
                KlipySourceType.Trending => _trendingItems,
                KlipySourceType.Search => _searchItems,
                KlipySourceType.Random => _randomItems,
                _ => _randomItems
            };

            dictionary[channel.ChannelId] = unseenGifWithSource.Data;
        }

        _logger.LogKlipyStageCount(_trendingItems.Count + _searchItems.Count + _randomItems.Count);
    }
}
