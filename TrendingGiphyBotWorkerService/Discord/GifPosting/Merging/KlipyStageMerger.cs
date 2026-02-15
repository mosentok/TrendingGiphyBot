using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Merging;

[RegisterSingleton]
public class KlipyStageMerger : IKlipyStageMerger
{
    public IImmutableDictionary<ulong, KlipyData> MergeStagesToDictionary(
        IImmutableDictionary<ulong, KlipyData> trending,
        IImmutableDictionary<ulong, KlipyData> search,
        IImmutableDictionary<ulong, KlipyData> random
    )
    {
        var merged = new Dictionary<ulong, KlipyData>();

        foreach (var entry in trending)
            merged[entry.Key] = entry.Value;

        foreach (var entry in search)
            merged[entry.Key] = entry.Value;

        foreach (var entry in random)
            merged[entry.Key] = entry.Value;

        return merged.ToImmutableDictionary();
    }
}