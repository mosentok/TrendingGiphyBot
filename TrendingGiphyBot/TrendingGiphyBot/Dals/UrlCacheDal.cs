using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public class UrlCacheDal : Dal
    {
        internal UrlCacheDal(string connectionString) : base(connectionString) { }
        internal Task<bool> Any(string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.Any(s => s.Url == url);
            });
        }
        internal Task<bool> Any()
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.Any();
            });
        }
        internal async Task Insert(string url)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                entities.UrlCaches.Add(new UrlCache { Url = url });
                await entities.SaveChangesAsync();
            }
        }
        internal Task<string> GetLatestUrl()
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.OrderByDescending(s => s.Stamp).FirstOrDefault()?.Url;
            });
        }
    }
}
