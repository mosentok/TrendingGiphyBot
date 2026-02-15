using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

[RegisterSingleton]
public class GiphySourceTypeResolver : IGiphySourceTypeResolver
{
    public GiphySourceSelection? TryGetFromBehaviorAndSource(
        GifPostingBehaviorKind behavior,
        GifSourceKind source,
        ulong channelId,
        string? gifKeyword,
        IImmutableDictionary<ulong, GiphyData> stagedGiphyTrending,
        IImmutableDictionary<ulong, GiphyData> stagedGiphySearch,
        IImmutableDictionary<ulong, GiphyData> stagedGiphyRandom
    )
    {
        if (source != GifSourceKind.Giphy)
            return null;

        switch (behavior)
        {
            case GifPostingBehaviorKind.TrendingGifsOnly:
                if (stagedGiphyTrending.TryGetValue(channelId, out var trendingData))
                    return new(trendingData, GiphySourceType.Trending);

                return null;

            case GifPostingBehaviorKind.TrendingGifsWithRandomGifs:
                if (stagedGiphyTrending.TryGetValue(channelId, out var trendingData2))
                    return new(trendingData2, GiphySourceType.Trending);

                if (!string.IsNullOrEmpty(gifKeyword) && stagedGiphySearch.TryGetValue(channelId, out var searchData))
                    return new(searchData, GiphySourceType.Search);

                if (stagedGiphyRandom.TryGetValue(channelId, out var randomData))
                    return new(randomData, GiphySourceType.Random);

                return null;

            default:
                return null;
        }
    }
}