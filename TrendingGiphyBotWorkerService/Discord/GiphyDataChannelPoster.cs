using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.GifPosting;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord;

[RegisterSingleton]
public class GiphyDataChannelPoster(
    ILogger<DiscordPostingWorker> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IDiscordSocketClientWrapper _discordSocketClientWrapper,
    IGiphyPostingEvictionHelper _evictionHelper
) : IGiphyDataChannelPoster
{
    public async Task PostGiphyGifsAsync(IImmutableDictionary<ulong, GiphyGifPostSelection> selections, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        // TODO parallelize this loop?
        foreach (var (channelId, selection) in selections)
        {
            var gifPost = new GiphyPost { ChannelId = channelId, GiphyDataId = selection.Data.Id };

            try
            {
                var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                if (channel is not IMessageChannel messageChannel)
                    throw new ThisShouldBeImpossibleException();

                trendingGiphyBotDbContext.GiphyPosts.Add(gifPost);

                await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                try
                {
                    var prefix = selection.SourceType == GiphySourceType.Trending ? "*Trending!* " : "";

                    await messageChannel.SendMessageAsync($"{prefix}{selection.Data.Url}");

                    _evictionHelper.EvictFromCorrectStage(channelId, selection.SourceType);
                }
                catch (Exception innerException)
                {
                    _logger.LogErrorPostingGiphy(innerException, selection.Data.Id, channelId, gifPost);

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
