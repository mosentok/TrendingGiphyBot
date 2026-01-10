namespace TrendingGiphyBotWorkerService.Giphy;

// TODO get rid of _items? unless we expect to seed the values somehow. that ought to be done post-DI by the worker already anyway tho
public class GifCache(List<GiphyData> _items, int _maxCount) : IGifCache
{
    public OrderedDictionary<string, GiphyData> Items { get; } = new(_items.ToDictionary(s => s.Id, s => s));

    public void Add(ICollection<GiphyData> gifs)
    {
        foreach (var gif in gifs)
            _ = Items.TryAdd(gif.Id, gif);

        while (Items.Count > _maxCount)
            Items.RemoveAt(0);
    }

    public GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen)
    {
        return Items.Where(s => !idsAlreadySeen.Contains(s.Key)).Select(s => s.Value).FirstOrDefault();
    }
}
