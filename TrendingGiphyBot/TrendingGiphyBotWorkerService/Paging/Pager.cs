namespace TrendingGiphyBotWorkerService.Paging;

public class Pager(PagerConfig _pagerConfig) : IPager
{
    public async Task<List<T>> PageAsync<T>(SearchWithOffset<T> searchWithOffsetAsync)
    {
        var allResults = new List<T>();

        var numberOfResponses = 0;

        for (var numberOfLoops = 0; numberOfLoops < _pagerConfig.MaxCacheLoops && numberOfResponses % _pagerConfig.MaxPageCount == 0; numberOfLoops++)
        {
            var searchResults = await searchWithOffsetAsync(numberOfResponses);

            allResults.AddRange(searchResults);

            numberOfResponses += searchResults.Count;
        }

        return allResults;
    }
}
