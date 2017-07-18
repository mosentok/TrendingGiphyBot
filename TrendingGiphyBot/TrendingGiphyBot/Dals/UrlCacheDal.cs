using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class UrlCacheDal : Dal
    {
        public UrlCacheDal(string connectionString) : base(connectionString) { }
        internal Task<bool> Any(string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.Any(s => s.Url == url);
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
        internal Task<UrlCache> Get(string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.SingleOrDefault(s => s.Url == url);
            });
        }
        internal Task<string> GetLatestUrl()
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.OrderByDescending(s => s.Stamp).FirstOrDefault().Url;
            });
        }
    }
}
