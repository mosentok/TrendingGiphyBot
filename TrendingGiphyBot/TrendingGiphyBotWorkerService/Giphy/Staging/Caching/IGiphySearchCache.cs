using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public interface IGiphySearchCache
{
    GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen);
    Task RefreshSearchedGiphyDatasAsync(string searchTerms, CancellationToken cancellationToken = default);
}
