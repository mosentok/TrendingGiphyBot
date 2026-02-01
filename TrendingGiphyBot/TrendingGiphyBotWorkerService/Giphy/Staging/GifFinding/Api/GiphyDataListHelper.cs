namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class GiphyDataListHelper : IGiphyDataListHelper
{
    public async Task<List<GiphyData>> NewMethod(int maxGiphyCacheLoops1, int maxPageCount1, Func<int, Task<GiphyResponse>> getGiphyResponseAsync)
    {
        var itemsToAdd = new List<GiphyData>();

        var numberOfResponses = 0;

        for (var numberOfLoops = 0; numberOfLoops < maxGiphyCacheLoops1 && numberOfResponses % maxPageCount1 == 0; numberOfLoops++)
        {
            var giphyResponse = await getGiphyResponseAsync(numberOfResponses);

            itemsToAdd.AddRange(giphyResponse.Data);

            numberOfResponses += giphyResponse.Data.Count;
        }

        return itemsToAdd;
    }

    public void AddToList(List<GiphyData> listToAddTo, List<GiphyData> newItems)
    {
        var alreadySeenGiphyDataIds = listToAddTo.Select(s => s.Id);
        var notInListYet = newItems.Where(s => !alreadySeenGiphyDataIds.Contains(s.Id)).ToArray();

        listToAddTo.AddRange(notInListYet);
    }

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
