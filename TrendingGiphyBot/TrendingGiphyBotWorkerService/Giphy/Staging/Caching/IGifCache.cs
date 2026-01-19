using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public interface IGifCache
{
    GiphyData? GetFirstUnseenGif();
    GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen);
    void Add(ICollection<GiphyData> gifs);
}
