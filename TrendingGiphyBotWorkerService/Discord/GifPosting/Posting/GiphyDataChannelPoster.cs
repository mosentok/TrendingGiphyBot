using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

[RegisterSingleton]
public class GiphyDataChannelPoster(
    ILogger<GiphyDataChannelPoster> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IDiscordSocketClientWrapper _discordSocketClientWrapper,
    IGiphyPostingEvictionHelper _evictionHelper
) : IGiphyDataChannelPoster
{
    public async Task PostGiphyGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, Dictionary<ulong, GiphySourceType> channelIdToSourceType, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        // TODO parallelize this loop?
        foreach (var channelId in channelIdToSourceType.Keys)
        {
            var gifPost = new GiphyPost
            {
                ChannelId = channelId,
                CreatedUtc = DateTime.UtcNow,
                GiphyDataId = stagedChannelGifPosts[channelId].Id
            };

            try
            {
                var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                if (channel is not IMessageChannel messageChannel)
                    throw new ThisShouldBeImpossibleException();

                trendingGiphyBotDbContext.GiphyPosts.Add(gifPost);

                await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                try
                {
                    var sourceType = channelIdToSourceType[channelId];
                    var prefix = sourceType == GiphySourceType.Trending ? "*Trending!* " : "";

                    await messageChannel.SendMessageAsync($"{prefix}{stagedChannelGifPosts[channelId].Url}");

                    _evictionHelper.EvictFromCorrectStage(channelId, sourceType);
                }
                catch (Exception innerException)
                {
                    _logger.LogErrorPostingGiphy(innerException, stagedChannelGifPosts[channelId].Id, channelId, gifPost);

                    trendingGiphyBotDbContext.GiphyPosts.Remove(gifPost);

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