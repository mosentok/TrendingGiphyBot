using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Random;

public interface IGiphyRandomCache
{
    GiphyData? GetFirstGif(string rating);
    GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen, string rating);
    Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default);
}
