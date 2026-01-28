using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public class Pager(PagerConfig _pagerConfig) : IPager
{
    public async Task<List<GiphyData>> PageAsync(SearchWithOffsetAsync searchWithOffsetAsync)
    {
        int totalCount;
        var numberOfLoops = 0;
        var numberOfResponses = 0;
        var allResults = new List<GiphyData>();

        do
        {
            var searchResults = await searchWithOffsetAsync(numberOfResponses);

            totalCount = searchResults.Pagination.TotalCount;
            numberOfResponses += searchResults.Pagination.Count;

            allResults.AddRange(searchResults.Data);

            numberOfLoops++;

        } while (numberOfResponses < totalCount && numberOfLoops < _pagerConfig.MaxCacheLoops);

        return allResults;
    }
}
