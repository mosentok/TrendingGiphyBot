using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;

[RegisterSingleton]
public class KlipyPager(IOptions<AppConfig> _appConfig) : IKlipyPager
{
    // TODO cancellation token
    public async Task<List<KlipyData>> PageAsync(SearchKlipyByPageAsync searchKlipyByPageAsync, CancellationToken cancellationToken = default)
    {
        var numberOfLoops = 0;
        var numberOfResponses = 0;
        var allResults = new List<KlipyData>();
        var page = 1;
        bool hasNext;

        do
        {
            var searchResults = await searchKlipyByPageAsync(page, cancellationToken);

            numberOfResponses += searchResults.Data.Data.Length;

            allResults.AddRange(searchResults.Data.Data);

            hasNext = searchResults.Data.HasNext;

            numberOfLoops++;
            page++;

        } while (hasNext && numberOfLoops < _appConfig.Value.Pager.MaxCacheLoops);

        return allResults;
    }
}

public delegate Task<KlipyResponse> SearchKlipyByPageAsync(int page, CancellationToken cancellationToken = default);
