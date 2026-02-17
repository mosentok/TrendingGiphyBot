using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Merging;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging;

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
    IServiceScopeFactory _serviceScopeFactory,
    IGiphyStageMerger _giphyStageMerger,
    IKlipyStageMerger _klipyStageMerger,
    IGifPostingBehaviorApplier _behaviorApplier
) : IGifPoster
{
    public async Task PostGifsAsync(CancellationToken stoppingToken)
    {
        var stagedGiphyTrending = _giphyDataStage.GetTrendingGiphyPostStage();
        var stagedGiphySearch = _giphyDataStage.GetSearchGiphyPostStage();
        var stagedGiphyRandom = _giphyDataStage.GetRandomGiphyPostStage();

        var stagedKlipyTrending = _klipyDataStage.GetTrendingKlipyPostStage();
        var stagedKlipySearch = _klipyDataStage.GetSearchKlipyPostStage();
        var stagedKlipyRandom = _klipyDataStage.GetRandomKlipyPostStage();

        var allGiphyData = _giphyStageMerger.MergeStagesToDictionary(stagedGiphyTrending, stagedGiphySearch, stagedGiphyRandom);
        var allKlipyData = _klipyStageMerger.MergeStagesToDictionary(stagedKlipyTrending, stagedKlipySearch, stagedKlipyRandom);

        _logger.LogStagedChannelGiphyPosts(allGiphyData);
        _logger.LogStagedChannelKlipyPosts(allKlipyData);

        var allStagedChannelIds = stagedGiphyTrending.Keys
            .Union(stagedGiphySearch.Keys)
            .Union(stagedGiphyRandom.Keys)
            .Union(stagedKlipyTrending.Keys)
            .Union(stagedKlipySearch.Keys)
            .Union(stagedKlipyRandom.Keys)
            .Distinct();

        var channelIdsInPostingHours = await _channelFinder.GetChannelSettingsIdsReadyToPostAsync(allStagedChannelIds, stoppingToken);

        _logger.LogChannelIdsInPostingHours(channelIdsInPostingHours);

        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var channelSettings = await trendingGiphyBotDbContext.ChannelSettings
            .Where(c => channelIdsInPostingHours.Contains(c.ChannelId))
            .ToListAsync(stoppingToken);

        var selections = _behaviorApplier.SelectGifsForPosting(
            stagedGiphyTrending, stagedGiphySearch, stagedGiphyRandom,
            stagedKlipyTrending, stagedKlipySearch, stagedKlipyRandom,
            channelSettings
        );

        await _giphyDataChannelPoster.PostGiphyGifsAsync(
            selections.GiphySelections,
            stoppingToken
        );

        await _klipyDataChannelPoster.PostKlipyGifsAsync(
            selections.KlipySelections,
            stoppingToken
        );
    }
}

