using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphySearchCache
{
    GiphyData? GetFirstGif(string searchTerm);
    GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen);
    Task RefreshSearchedGiphyDatasAsync(string searchTerms, CancellationToken cancellationToken = default);
}
