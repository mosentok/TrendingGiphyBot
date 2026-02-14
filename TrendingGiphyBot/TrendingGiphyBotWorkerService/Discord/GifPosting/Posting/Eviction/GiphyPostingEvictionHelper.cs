using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;

[RegisterSingleton]
public class GiphyPostingEvictionHelper(IGiphyDataStage _giphyDataStage) : IGiphyPostingEvictionHelper
{
    public void EvictFromCorrectStage(ulong channelId, GiphySourceType sourceType)
    {
        switch (sourceType)
        {
            case GiphySourceType.Trending:
                _giphyDataStage.EvictTrending(channelId);
                break;
            case GiphySourceType.Search:
                _giphyDataStage.EvictSearch(channelId);
                break;
            case GiphySourceType.Random:
                _giphyDataStage.EvictRandom(channelId);
                break;
        }
    }
}