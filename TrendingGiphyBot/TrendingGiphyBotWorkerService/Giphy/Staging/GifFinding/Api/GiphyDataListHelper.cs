namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

[RegisterSingleton]
public class GiphyDataListHelper : IGiphyDataListHelper
{
    public void SortToMaxSize(List<GiphyData> listToSort, List<GiphyData> itemsToAdd, int maxCount)
    {
        var alreadySeenGiphyDataIds = listToSort.Select(s => s.Id);
        var notInListYet = itemsToAdd.Where(s => !alreadySeenGiphyDataIds.Contains(s.Id)).ToArray();

        listToSort.AddRange(notInListYet);

        listToSort.Sort((left, right) => string.Compare(left.TrendingDatetime, right.TrendingDatetime));

        if (listToSort.Count > maxCount)
        {
            var excessCount = listToSort.Count - maxCount;

            listToSort.RemoveRange(maxCount, excessCount);
        }
    }
}
