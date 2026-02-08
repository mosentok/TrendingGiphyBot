using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Discord.Delaying;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Logging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker
(
    ILogger<DiscordPostingWorker> _logger,
    IGiphyDataStage _giphyDataStage,
    IKlipyDataStage _klipyDataStage,
    IDelayer _delayer,
    IGiphyDataChannelPoster _discordChannelGifPoster,
    IKlipyDataChannelPoster _discordKlipyChannelGifPoster,
    IChannelSettingsFinder _channelFinder,
    IServiceScopeFactory _serviceScopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

                _logger.LogPostingGifs();

                var giphyStaged = _giphyDataStage.GetChannelGifPostStage();
                var klipyStaged = _klipyDataStage.GetChannelGifPostStage();

                _logger.LogStagedChannelGifPosts(giphyStaged);

                var allStagedChannelIds = giphyStaged.Keys.Union(klipyStaged.Keys).Distinct();

                var channelIdsInPostingHours = await _channelFinder.GetChannelSettingsIdsReadyToPostAsync(allStagedChannelIds, stoppingToken);

                _logger.LogChannelIdsInPostingHours(channelIdsInPostingHours);

                using var scope = _serviceScopeFactory.CreateScope();

                var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

                var channelSettings = await trendingGiphyBotDbContext.ChannelSettings
                    .Where(c => channelIdsInPostingHours.Contains(c.ChannelId))
                    .ToListAsync(stoppingToken);

                var (giphyChannelIds, klipyChannelIds) = SeparateChannelIds(giphyStaged, klipyStaged, channelSettings);

                await _discordChannelGifPoster.PostGifsAsync(giphyStaged, giphyChannelIds, stoppingToken);
                await _discordKlipyChannelGifPoster.PostKlipyGifsAsync(klipyStaged, klipyChannelIds, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogTopLevelException(ex);
            }
            finally
            {
                _logger.LogPostedGifs();
            }
    }

    private static (List<ulong> GiphyToPost, List<ulong> KlipyToPost) SeparateChannelIds(IImmutableDictionary<ulong, GiphyData> giphyStaged, IImmutableDictionary<ulong, KlipyData> klipyStaged, List<ChannelSettingsModel> channelSettings)
    {
        var giphyToPost = new List<ulong>();
        var klipyToPost = new List<ulong>();

        foreach (var channelSetting in channelSettings)
        {
            var enabledGifSources = DetermineEnabledGifSources(channelSetting);

            var shuffledGifSources = enabledGifSources.OrderBy(_ => Random.Shared.Next());

            foreach (var source in shuffledGifSources)
            {
                if (source == GifSourceKind.Giphy && giphyStaged.ContainsKey(channelSetting.ChannelId))
                {
                    giphyToPost.Add(channelSetting.ChannelId);

                    break;
                }

                if (source == GifSourceKind.Klipy && klipyStaged.ContainsKey(channelSetting.ChannelId))
                {
                    klipyToPost.Add(channelSetting.ChannelId);

                    break;
                }
            }
        }

        return (giphyToPost, klipyToPost);
    }

    static List<GifSourceKind> DetermineEnabledGifSources(ChannelSettingsModel channelSetting)
    {
        if (channelSetting.GifSource is not { } gifSource)
            return [.. Enum.GetValues<GifSourceKind>()];

        var enabled = new List<GifSourceKind>();

        if (gifSource.HasFlag(GifSourceKind.Giphy))
            enabled.Add(GifSourceKind.Giphy);

        if (gifSource.HasFlag(GifSourceKind.Klipy))
            enabled.Add(GifSourceKind.Klipy);

        return enabled;
    }
}
