using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Resolving;

public record KlipySourceSelection(
    KlipyData Data,
    KlipySourceType SourceType
);
