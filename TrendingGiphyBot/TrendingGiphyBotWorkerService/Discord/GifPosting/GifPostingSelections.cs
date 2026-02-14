using System.Collections.Immutable;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public record GifPostingSelections(
    IImmutableDictionary<ulong, GiphyGifPostSelection> GiphySelections,
    IImmutableDictionary<ulong, KlipyGifPostSelection> KlipySelections
);
