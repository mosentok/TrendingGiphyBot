namespace TrendingGiphyBotWorkerService.Giphy;

public interface IGifCache
{
    void Add(ICollection<GiphyData> gifs);

    List<GiphyData> Items { get; }
}
