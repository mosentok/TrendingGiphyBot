using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphyTrendingCache
{
    GiphyData? GetFirstGif(string rating);
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen, string rating);
    Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default);
}
