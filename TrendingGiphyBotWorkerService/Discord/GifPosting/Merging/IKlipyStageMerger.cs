using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Merging;

public interface IKlipyStageMerger
{
    IImmutableDictionary<ulong, KlipyData> MergeStagesToDictionary(
        IImmutableDictionary<ulong, KlipyData> trending,
        IImmutableDictionary<ulong, KlipyData> search,
        IImmutableDictionary<ulong, KlipyData> random
    );
}