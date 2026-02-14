using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

[RegisterSingleton]
public class GifPostingBehaviorApplier(
    IEnabledGifSourceDeterminer _enabledGifSourceDeterminer,
    IGiphySourceTypeResolver _giphySourceTypeResolver,
    IKlipySourceTypeResolver _klipySourceTypeResolver
) : IGifPostingBehaviorApplier
{
    public GifPostingSelections SelectGifsForPosting(
        IImmutableDictionary<ulong, GiphyData> stagedGiphyTrending,
        IImmutableDictionary<ulong, GiphyData> stagedGiphySearch,
        IImmutableDictionary<ulong, GiphyData> stagedGiphyRandom,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyTrending,
        IImmutableDictionary<ulong, KlipyData> stagedKlipySearch,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyRandom,
        List<ChannelSettingsModel> channelSettings
    )
    {
        var giphySelections = new Dictionary<ulong, GiphyGifPostSelection>();
        var klipySelections = new Dictionary<ulong, KlipyGifPostSelection>();

        foreach (var channelSetting in channelSettings)
        {
            var behavior = (GifPostingBehaviorKind)channelSetting.GifPostingBehaviorId;

            var enabledSources = _enabledGifSourceDeterminer.DetermineEnabledGifSources(channelSetting);
            var shuffledSources = enabledSources.OrderBy(_ => Random.Shared.Next());

            foreach (var source in shuffledSources)
            {
                var giphySelection = _giphySourceTypeResolver.TryGetFromBehaviorAndSource(
                    behavior,
                    source,
                    channelSetting.ChannelId,
                    channelSetting.GifKeyword,
                    stagedGiphyTrending, stagedGiphySearch, stagedGiphyRandom
                );

                if (giphySelection is not null)
                {
                    giphySelections[channelSetting.ChannelId] = new(giphySelection.Data, giphySelection.SourceType);

                    break;
                }

                var klipySelection = _klipySourceTypeResolver.TryGetFromBehaviorAndSource(
                    behavior,
                    source,
                    channelSetting.ChannelId,
                    channelSetting.GifKeyword,
                    stagedKlipyTrending, stagedKlipySearch, stagedKlipyRandom
                );

                if (klipySelection is not null)
                {
                    klipySelections[channelSetting.ChannelId] = new(klipySelection.Data, klipySelection.SourceType);

                    break;
                }
            }
        }

        return new(
            giphySelections.ToImmutableDictionary(),
            klipySelections.ToImmutableDictionary()
        );
    }
}
