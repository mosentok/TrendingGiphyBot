namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public interface IGiphyDataListHelper
{
    Task<List<GiphyData>> NewMethod(int maxGiphyCacheLoops1, int maxPageCount1, Func<int, Task<GiphyResponse>> getGiphyResponseAsync);
    void AddToList(List<GiphyData> listToAddTo, List<GiphyData> newItems);
    void SortToMaxSize(List<GiphyData> listToSort, List<GiphyData> itemsToAdd, int maxCount);
}
