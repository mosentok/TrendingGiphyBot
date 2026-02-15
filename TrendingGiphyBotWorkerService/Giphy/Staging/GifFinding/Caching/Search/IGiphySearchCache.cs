using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Search;

public interface IGiphySearchCache
{
    GiphyData? GetFirstGif(string searchTerm, string rating);
    GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen, string rating);
    Task RefreshSearchedGiphyDatasAsync(string searchTerms, CancellationToken cancellationToken = default);
}
