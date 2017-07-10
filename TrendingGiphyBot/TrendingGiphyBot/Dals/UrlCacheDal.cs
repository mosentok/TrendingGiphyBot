using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class UrlCacheDal
    {
        readonly string _ConnectionString;
        public UrlCacheDal(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        internal Task<bool> Any(short minute)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                    return dataContext.UrlCaches.Any(s => s.Minute == minute);
            });
        }
        internal Task Insert(UrlCache urlCache)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                {
                    dataContext.UrlCaches.InsertOnSubmit(urlCache);
                    dataContext.SubmitChanges();
                }
            });
        }
        internal Task Update(UrlCache urlCache)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                {
                    var match = dataContext.UrlCaches.Single(s => s.Minute == urlCache.Minute);
                    match.Url = urlCache.Url;
                    dataContext.SubmitChanges();
                }
            });
        }
        internal Task<UrlCache> Get(short minute)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                    return dataContext.UrlCaches.SingleOrDefault(s => s.Minute == minute);
            });
        }
    }
}
