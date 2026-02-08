using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class KlipyDataListHelper : IKlipyDataListHelper
{
    public void TrimToMaxSize(List<KlipyData> listToTrim, List<KlipyData> itemsToAdd, int maxCount)
    {
        var alreadySeenIds = listToTrim.Select(s => s.Id);
        var notInListYet = itemsToAdd.Where(s => !alreadySeenIds.Contains(s.Id)).ToArray();

        listToTrim.AddRange(notInListYet);

        if (listToTrim.Count > maxCount)
        {
            var excessCount = listToTrim.Count - maxCount;

            listToTrim.RemoveRange(0, excessCount);
        }
    }
}
