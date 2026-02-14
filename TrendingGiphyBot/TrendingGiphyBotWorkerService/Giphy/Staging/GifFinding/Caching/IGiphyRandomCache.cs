using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphyRandomCache
{
    GiphyData? GetFirstGif(string rating);
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen, string rating);
    Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default);
}
