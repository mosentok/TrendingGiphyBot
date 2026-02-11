using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

// TODO combine this into one stage so the database doesn't have to be requeried
[RegisterSingleton]
public class KlipyDataStage(
    ILogger<KlipyDataStage> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IKlipyDataFinder _klipyDataFinder
) : IKlipyDataStage
{
    readonly Dictionary<ulong, KlipyData> _items = [];

    public IImmutableDictionary<ulong, KlipyData> GetChannelKlipyPostStage() => _items.ToImmutableDictionary();

    public void Evict(ulong channelId) => _items.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.KlipyPosts)
            .Where(s => !_items.Keys.Contains(s.ChannelId) && s.Frequency > 0)
            .ToAsyncEnumerable();

        await foreach (var channel in activeChannels)
        {
            var maybe = _klipyDataFinder.TryGetUnseenGif(channel, cancellationToken);

            if (maybe is not null)
                _items[channel.ChannelId] = maybe;
        }

        _logger.LogKlipyStageCount(_items.Count);
    }
}
