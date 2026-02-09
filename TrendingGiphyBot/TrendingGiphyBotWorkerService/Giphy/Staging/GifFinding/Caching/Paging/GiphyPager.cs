using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

[RegisterSingleton]
public class GiphyPager(
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyPager
{
    // TODO cancellation token
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

        } while (numberOfResponses < totalCount && numberOfLoops < _appConfig.CurrentValue.Pager.MaxCacheLoops);

        return allResults;
    }
}
