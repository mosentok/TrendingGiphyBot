using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Merging;

public interface IGiphyStageMerger
{
    IImmutableDictionary<ulong, GiphyData> MergeStagesToDictionary(
        IImmutableDictionary<ulong, GiphyData> trending,
        IImmutableDictionary<ulong, GiphyData> search,
        IImmutableDictionary<ulong, GiphyData> random
    );
}