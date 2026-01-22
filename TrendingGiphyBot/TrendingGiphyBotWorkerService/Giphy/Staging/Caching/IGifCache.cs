using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public interface IGifCache
{
    Task RefreshAsync(CancellationToken cancellationToken = default);
    GiphyData? GetFirstGif();
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen);
}
