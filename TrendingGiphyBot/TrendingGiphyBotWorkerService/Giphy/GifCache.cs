namespace TrendingGiphyBotWorkerService.Giphy;

public class GifCache(List<GiphyData> _items, int _maxCount) : IGifCache
{
    public List<GiphyData> Items { get; } = [.. _items];

    public void Add(ICollection<GiphyData> gifs)
    {
        foreach (var gif in gifs)
        {
            var isADuplicate = Items.Any(s => s.Id == gif.Id);

            if (!isADuplicate)
                Items.Add(gif);
        }

        while (Items.Count > _maxCount)
            Items.RemoveAt(0);
    }
}