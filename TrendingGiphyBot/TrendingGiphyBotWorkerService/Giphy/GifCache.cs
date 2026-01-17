namespace TrendingGiphyBotWorkerService.Giphy;

// TODO get rid of _items? unless we expect to seed the values somehow. that ought to be done post-DI by the worker already anyway tho
public class GifCache(GifCacheConfig _gifCacheConfig) : IGifCache
{
    public List<GiphyData> Items { get; } = [.. _gifCacheConfig.Items.OrderByDescending(s => s.TrendingDatetime)];

    public void Add(ICollection<GiphyData> gifs)
    {
        var notInListYet = gifs.Where(s => !Items.Contains(s));

        Items.AddRange(notInListYet);
        Items.Sort((left, right) => string.Compare(left.TrendingDatetime, right.TrendingDatetime));

        if (Items.Count <= _gifCacheConfig.MaxCount)
            return;

        var excessCount = Items.Count - _gifCacheConfig.MaxCount;

        Items.RemoveRange(_gifCacheConfig.MaxCount, excessCount);
    }

    public GiphyData? GetFirstUnseenGif() => Items.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen) => Items.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
