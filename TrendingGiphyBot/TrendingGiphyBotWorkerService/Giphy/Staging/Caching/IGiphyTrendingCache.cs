using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public interface IGiphyTrendingCache
{
    GiphyData? GetFirstGif();
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen);
    Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default);
}
