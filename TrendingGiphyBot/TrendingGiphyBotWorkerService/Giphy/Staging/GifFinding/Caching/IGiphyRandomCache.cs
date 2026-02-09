using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphyRandomCache
{
    GiphyData? GetFirstGif();
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen);
    Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default);
}
