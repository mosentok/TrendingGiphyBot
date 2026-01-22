using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifPostStage(IServiceScopeFactory _serviceScopeFactory, IGifCache _gifCache, IGiphyClient _giphyClient, GifPostStageConfig _gifPostStageConfig) : IGifPostStage
{
    readonly Dictionary<ulong, GiphyData> _channelGifPostStage = [];

    public IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage() => _channelGifPostStage.ToImmutableDictionary();

    public void Evict(ulong channelId) => _channelGifPostStage.Remove(channelId);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var activeChannels = await trendingGiphyBotDbContext.ChannelSettings
            .Include(s => s.GifPosts)
            .Where(s => !_channelGifPostStage.Keys.Contains(s.ChannelId) && s.Frequency > 0)
            .ToListAsync(cancellationToken);

        foreach (var channel in activeChannels)
        {
            if (channel.GifPosts is null or { Count: 0 })
            {
                var firstGif = _gifCache.GetFirstGif();

                if (firstGif is null)
                    continue;

                _channelGifPostStage[channel.ChannelId] = firstGif;

                continue;
            }

            var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
            var firstUnseenGif = _gifCache.GetFirstUnseenGif(seenGiphyDataIds);

            if (firstUnseenGif is not null)
            {
                _channelGifPostStage[channel.ChannelId] = firstUnseenGif;

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
                    _channelGifPostStage[channel.ChannelId] = randomGif.Data;

                    break;
                }

                attempts++;
            }
            while (attempts < _gifPostStageConfig.MaxRandomGifAttempts);
        }
    }
}