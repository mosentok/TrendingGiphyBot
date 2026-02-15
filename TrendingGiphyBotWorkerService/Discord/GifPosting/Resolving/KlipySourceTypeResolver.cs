using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

[RegisterSingleton]
public class KlipySourceTypeResolver : IKlipySourceTypeResolver
{
    public KlipySourceSelection? TryGetFromBehaviorAndSource(
        GifPostingBehaviorKind behavior,
        GifSourceKind source,
        ulong channelId,
        string? gifKeyword,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyTrending,
        IImmutableDictionary<ulong, KlipyData> stagedKlipySearch,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyRandom
    )
    {
        if (source != GifSourceKind.Klipy)
            return null;

        switch (behavior)
        {
            case GifPostingBehaviorKind.TrendingGifsOnly:
                if (stagedKlipyTrending.TryGetValue(channelId, out var trendingData))
                    return new(trendingData, KlipySourceType.Trending);

                return null;

            case GifPostingBehaviorKind.TrendingGifsWithRandomGifs:
                if (stagedKlipyTrending.TryGetValue(channelId, out var trendingData2))
                    return new(trendingData2, KlipySourceType.Trending);

                if (!string.IsNullOrEmpty(gifKeyword) && stagedKlipySearch.TryGetValue(channelId, out var searchData))
                    return new(searchData, KlipySourceType.Search);

                if (stagedKlipyRandom.TryGetValue(channelId, out var randomData))
                    return new(randomData, KlipySourceType.Random);

                return null;

            default:
                return null;
        }
    }
}