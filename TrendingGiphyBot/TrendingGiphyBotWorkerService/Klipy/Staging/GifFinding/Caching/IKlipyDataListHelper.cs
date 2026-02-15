using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipyDataListHelper
{
    void TrimToMaxSize(List<KlipyData> listToTrim, List<KlipyData> itemsToAdd, int maxCount);
}