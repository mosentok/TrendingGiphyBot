namespace TrendingGiphyBotWorkerService.Giphy;

public interface IGifCache
{
    GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen);
    void Add(ICollection<GiphyData> gifs);
}
