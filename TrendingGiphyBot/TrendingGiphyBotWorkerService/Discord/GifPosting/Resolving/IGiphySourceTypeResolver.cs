using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

public interface IGiphySourceTypeResolver
{
    GiphySourceSelection? TryGetFromBehaviorAndSource(
        GifPostingBehaviorKind behavior,
        GifSourceKind source,
        ulong channelId,
        string? gifKeyword,
        IImmutableDictionary<ulong, GiphyData> stagedGiphyTrending,
        IImmutableDictionary<ulong, GiphyData> stagedGiphySearch,
        IImmutableDictionary<ulong, GiphyData> stagedGiphyRandom
    );
}