using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifPostStage(
    ILogger<GifPostStage> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGifCache _gifCache,
    IGiphyClient _giphyClient,
    GifPostStageConfig _gifPostStageConfig
) : IGifPostStage
{
    readonly Dictionary<ulong, GiphyData> _items = [];

    public IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage() => _items.ToImmutableDictionary();

    public void Evict(ulong channelId) => _items.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogGifStageIsRefreshing();

        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = await trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.GifPosts)
            .Where(s => !_items.Keys.Contains(s.ChannelId) && s.Frequency > 0)
            .ToListAsync(cancellationToken);

        foreach (var channel in activeChannels)
        {
            if (channel.GifPosts is null or { Count: 0 })
            {
                var firstGif = _gifCache.GetFirstGif();

                if (firstGif is null)
                    continue;

                _items[channel.ChannelId] = firstGif;

                continue;
            }

            var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
            var firstUnseenGif = _gifCache.GetFirstUnseenGif(seenGiphyDataIds);

            if (firstUnseenGif is not null)
            {
                _items[channel.ChannelId] = firstUnseenGif;

                continue;
            }

            if (channel.GifPostingBehaviorId != GifPostingBehaviorKind.TrendingGifsWithRandomGifs.AsInt())
                continue;

            var attempts = 0;

            do
            {
                var randomGif = await _giphyClient.GetRandomGifAsync(channel.GifKeyword, cancellationToken: cancellationToken);

                var randomGifHasAlreadyBeenSeen = seenGiphyDataIds.Contains(randomGif.Data.Id);

                if (!randomGifHasAlreadyBeenSeen)
                {
                    _items[channel.ChannelId] = randomGif.Data;

                    break;
                }

                attempts++;
            }
            while (attempts < _gifPostStageConfig.MaxRandomGifAttempts);
        }

        _logger.LogGifStageHasRefreshed(_items.Count);
    }
}