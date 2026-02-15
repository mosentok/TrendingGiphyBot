using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;

public interface IKlipyPager
{
    Task<List<KlipyData>> PageAsync(SearchKlipyByPageAsync searchKlipyByPageAsync, CancellationToken cancellationToken = default);
}