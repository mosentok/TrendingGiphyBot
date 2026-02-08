using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.GifPosting;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

[RegisterSingleton]
public class GiphyDataChannelPoster(
    ILogger<DiscordPostingWorker> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGiphyDataStage _gifPostStage,
    IDiscordSocketClientWrapper _discordSocketClientWrapper
) : IGiphyDataChannelPoster
{
    public async Task PostGiphyGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        // TODO parallelize this loop?
        foreach (var channelId in channelIds)
        {
            var gifPost = new GiphyPost { ChannelId = channelId, GiphyDataId = stagedChannelGifPosts[channelId].Id };

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
                    _logger.LogErrorPostingGiphy(innerException, stagedChannelGifPosts[channelId].Id, channelId, gifPost);

                    trendingGiphyBotDbContext.GifPosts.Remove(gifPost);

                    await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogGiphyPostingException(ex, gifPost);
            }
        }
    }
}
