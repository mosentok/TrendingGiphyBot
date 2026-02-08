namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public interface IGiphyDataListHelper
{
    void SortToMaxSize(List<GiphyData> listToSort, List<GiphyData> itemsToAdd, int maxCount);
}
