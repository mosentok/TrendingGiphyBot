using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

public interface IKlipySourceTypeResolver
{
    KlipySourceSelection? TryGetFromBehaviorAndSource(
        GifPostingBehaviorKind behavior,
        GifSourceKind source,
        ulong channelId,
        string? gifKeyword,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyTrending,
        IImmutableDictionary<ulong, KlipyData> stagedKlipySearch,
        IImmutableDictionary<ulong, KlipyData> stagedKlipyRandom
    );
}