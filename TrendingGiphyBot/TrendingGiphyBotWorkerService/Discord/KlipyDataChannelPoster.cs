using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

[RegisterSingleton]
public class KlipyDataChannelPoster(
    ILogger<KlipyDataChannelPoster> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IKlipyDataStage _klipyDataStage,
    IDiscordSocketClientWrapper _discordSocketClientWrapper
) : IKlipyDataChannelPoster
{
    public async Task PostKlipyGifsAsync(IImmutableDictionary<ulong, KlipyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        foreach (var channelId in channelIds)
        {
            var klipyPost = new KlipyPost { ChannelId = channelId, KlipyDataId = stagedChannelGifPosts[channelId].Id };

            try
            {
                var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                if (channel is not IMessageChannel messageChannel)
                    throw new ThisShouldBeImpossibleException();

                trendingGiphyBotDbContext.KlipyPosts.Add(klipyPost);

                await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                try
                {
                    var url = stagedChannelGifPosts[channelId].File.Hd.Gif.Url;

                    await messageChannel.SendMessageAsync($"*Trending!* {url}");

                    _klipyDataStage.Evict(channelId);
                }
                catch (Exception innerException)
                {
                    _logger.LogErrorPostingKlipy(innerException, stagedChannelGifPosts[channelId].Id, channelId, klipyPost);

                    trendingGiphyBotDbContext.KlipyPosts.Remove(klipyPost);

                    await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogKlipyPostingException(ex, klipyPost);
            }
        }
    }
}

public interface IKlipyDataChannelPoster
{
    Task PostKlipyGifsAsync(IImmutableDictionary<ulong, KlipyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken);
}
