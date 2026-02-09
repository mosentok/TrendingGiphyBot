namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public interface IGiphyDataListHelper
{
    void TrimToMaxSize(List<GiphyData> items, int maxCount);
    void SortToMaxSize(List<GiphyData> listToSort, List<GiphyData> itemsToAdd, int maxCount);
}
