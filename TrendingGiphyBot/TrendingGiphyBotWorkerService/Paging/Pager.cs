using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public class Pager(IOptions<AppConfig> _appConfig) : IPager
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

        } while (numberOfResponses < totalCount && numberOfLoops < _appConfig.Value.Pager.MaxCacheLoops);

        return allResults;
    }
}
