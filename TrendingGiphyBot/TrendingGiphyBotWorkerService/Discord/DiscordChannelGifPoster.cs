using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordChannelGifPoster(
    ILogger<DiscordPostingWorker> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGifPostStage _gifPostStage,
    IDiscordSocketClientWrapper _discordSocketClientWrapper
) : IDiscordChannelGifPoster
{
    public async Task PostGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        // TODO parallelize this loop?
        foreach (var channelId in channelIds)
        {
            var gifPost = new GifPost { ChannelId = channelId, GiphyDataId = stagedChannelGifPosts[channelId].Id };

            try
            {
                var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                if (channel is not IMessageChannel messageChannel)
                    throw new ThisShouldBeImpossibleException();

                trendingGiphyBotDbContext.GifPosts.Add(gifPost);

                await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                try
                {
                    await messageChannel.SendMessageAsync($"*Trending!* {stagedChannelGifPosts[channelId].Url}");

                    _gifPostStage.Evict(channelId);
                }
                catch (Exception innerException)
                {
                    _logger.LogErrorPostingGif(innerException, stagedChannelGifPosts[channelId].Id, channelId, gifPost);

                    trendingGiphyBotDbContext.GifPosts.Remove(gifPost);

                    await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogGifPostingException(ex, gifPost);
            }
        }
    }
}
