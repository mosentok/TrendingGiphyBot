using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Merging;

[RegisterSingleton]
public class GiphyStageMerger : IGiphyStageMerger
{
    public IImmutableDictionary<ulong, GiphyData> MergeStagesToDictionary(
        IImmutableDictionary<ulong, GiphyData> trending,
        IImmutableDictionary<ulong, GiphyData> search,
        IImmutableDictionary<ulong, GiphyData> random
    )
    {
        var merged = new Dictionary<ulong, GiphyData>();

        foreach (var entry in trending)
            merged[entry.Key] = entry.Value;

        foreach (var entry in search)
            merged[entry.Key] = entry.Value;

        foreach (var entry in random)
            merged[entry.Key] = entry.Value;

        return merged.ToImmutableDictionary();
    }
}