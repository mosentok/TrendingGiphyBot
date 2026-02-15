using System.Collections.Immutable;
using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

[RegisterSingleton]
public class KlipyDataChannelPoster(
    ILogger<KlipyDataChannelPoster> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IDiscordSocketClientWrapper _discordSocketClientWrapper,
    IKlipyPostingEvictionHelper _evictionHelper
) : IKlipyDataChannelPoster
{
    public async Task PostKlipyGifsAsync(IImmutableDictionary<ulong, KlipyGifPostSelection> selections, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        foreach (var (channelId, selection) in selections)
        {
            var klipyPost = new KlipyPost
            {
                ChannelId = channelId,
                CreatedUtc = DateTime.UtcNow,
                KlipyDataId = selection.Data.Id
            };

            try
            {
                var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                if (channel is not IMessageChannel messageChannel)
                    throw new ThisShouldBeImpossibleException();

                trendingGiphyBotDbContext.KlipyPosts.Add(klipyPost);

                await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                try
                {
                    var url = selection.Data.File.Hd.Gif.Url;
                    var prefix = selection.SourceType == KlipySourceType.Trending ? "*Trending!* " : "";

                    await messageChannel.SendMessageAsync($"{prefix}{url}");

                    _evictionHelper.EvictFromCorrectStage(channelId, selection.SourceType);
                }
                catch (Exception innerException)
                {
                    _logger.LogErrorPostingKlipy(innerException, selection.Data.Id, channelId, klipyPost);

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