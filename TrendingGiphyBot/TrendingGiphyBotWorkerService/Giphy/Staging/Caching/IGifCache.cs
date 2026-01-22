using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public interface IGifCache
{
    Task RefreshAsync(CancellationToken cancellationToken = default);
    GiphyData? GetFirstUnseenGif();
    GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen);
}
