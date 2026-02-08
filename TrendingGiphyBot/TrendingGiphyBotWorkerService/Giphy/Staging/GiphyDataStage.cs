using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;


// TODO combine this into one stage so the database doesn't have to be requeried
[RegisterSingleton]
public class GiphyDataStage(
    ILogger<GiphyDataStage> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGiphyDataFinder _giphyDataFinder
) : IGiphyDataStage
{
    readonly Dictionary<ulong, GiphyData> _items = [];

    public IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage() => _items.ToImmutableDictionary();

    public void Evict(ulong channelId) => _items.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.GiphyPosts)
            .Where(s => !_items.Keys.Contains(s.ChannelId) && s.Frequency > 0)
            .ToAsyncEnumerable();

        await foreach (var channel in activeChannels)
        {
            var maybe = await _giphyDataFinder.TryGetUnseenGifAsync(channel, cancellationToken);

            if (maybe is { Success: true, Result: { } result })
                _items[channel.ChannelId] = result;
        }

        _logger.LogGifStageCount(_items.Count);
    }
}
