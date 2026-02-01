using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphyTrendingCache
{
    GiphyData? GetFirstGif();
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen);
    Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default);
}
