using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;

public interface IKlipyPostingEvictionHelper
{
    void EvictFromCorrectStage(ulong channelId, KlipySourceType sourceType);
}