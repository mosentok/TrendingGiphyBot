using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;

public interface IGiphyPostingEvictionHelper
{
    void EvictFromCorrectStage(ulong channelId, GiphySourceType sourceType);
}