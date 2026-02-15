using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphyDataListHelper
{
    void TrimToMaxSize(List<GiphyData> items, int maxCount);
    void SortToMaxSize(List<GiphyData> listToSort, List<GiphyData> itemsToAdd, int maxCount);
}
