using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

[RegisterSingleton]
public class GifPoster
(
    ILogger<GifPoster> _logger,
    IGiphyDataStage _giphyDataStage,
    IKlipyDataStage _klipyDataStage,
    IGiphyDataChannelPoster _giphyDataChannelPoster,
    IKlipyDataChannelPoster _klipyDataChannelPoster,
    IChannelSettingsFinder _channelFinder,
    IServiceScopeFactory _serviceScopeFactory
) : IGifPoster
{
    public async Task PostGifsAsync(CancellationToken stoppingToken)
    {
        var stagedGiphyData = _giphyDataStage.GetChannelGiphyPostStage();
        var stagedKlipyData = _klipyDataStage.GetChannelKlipyPostStage();

        _logger.LogStagedChannelGiphyPosts(stagedGiphyData);
        _logger.LogStagedChannelKlipyPosts(stagedKlipyData);

        var allStagedChannelIds = stagedGiphyData.Keys.Union(stagedKlipyData.Keys).Distinct();

        var channelIdsInPostingHours = await _channelFinder.GetChannelSettingsIdsReadyToPostAsync(allStagedChannelIds, stoppingToken);

        _logger.LogChannelIdsInPostingHours(channelIdsInPostingHours);

        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var channelSettings = await trendingGiphyBotDbContext.ChannelSettings
            .Where(c => channelIdsInPostingHours.Contains(c.ChannelId))
            .ToListAsync(stoppingToken);

        var (channelIdsToReceiveGiphy, channelIdsToReceiveKlipy) = SeparateChannelIds(stagedGiphyData, stagedKlipyData, channelSettings);

        // TODO this could probably be designed better
        await _giphyDataChannelPoster.PostGiphyGifsAsync(stagedGiphyData, channelIdsToReceiveGiphy, stoppingToken);
        await _klipyDataChannelPoster.PostKlipyGifsAsync(stagedKlipyData, channelIdsToReceiveKlipy, stoppingToken);
    }

    private static (List<ulong> ChannelIdsToReceiveGiphy, List<ulong> ChannelIdsToReceiveKlipy) SeparateChannelIds(IImmutableDictionary<ulong, GiphyData> stagedGiphyData, IImmutableDictionary<ulong, KlipyData> stagedKlipyData, List<ChannelSettingsModel> channelSettings)
    {
        var channelIdsToReceiveGiphy = new List<ulong>();
        var channelIdsToReceiveKlipy = new List<ulong>();

        foreach (var channelSetting in channelSettings)
        {
            var enabledGifSources = DetermineEnabledGifSources(channelSetting);

            var shuffledGifSources = enabledGifSources.OrderBy(_ => Random.Shared.Next());

            foreach (var source in shuffledGifSources)
            {
                if (source == GifSourceKind.Giphy && stagedGiphyData.ContainsKey(channelSetting.ChannelId))
                {
                    channelIdsToReceiveGiphy.Add(channelSetting.ChannelId);

                    break;
                }

                if (source == GifSourceKind.Klipy && stagedKlipyData.ContainsKey(channelSetting.ChannelId))
                {
                    channelIdsToReceiveKlipy.Add(channelSetting.ChannelId);

                    break;
                }
            }
        }

        return (channelIdsToReceiveGiphy, channelIdsToReceiveKlipy);
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
