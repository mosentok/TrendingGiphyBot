using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting.Eviction;

[RegisterSingleton]
public class KlipyPostingEvictionHelper(IKlipyDataStage _klipyDataStage) : IKlipyPostingEvictionHelper
{
    public void EvictFromCorrectStage(ulong channelId, KlipySourceType sourceType)
    {
        switch (sourceType)
        {
            case KlipySourceType.Trending:
                _klipyDataStage.EvictTrending(channelId);
                break;
            case KlipySourceType.Search:
                _klipyDataStage.EvictSearch(channelId);
                break;
            case KlipySourceType.Random:
                _klipyDataStage.EvictRandom(channelId);
                break;
        }
    }
}