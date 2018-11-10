using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public class UrlCacheDal : Dal
    {
        internal UrlCacheDal(string connectionString) : base(connectionString) { }
        internal async Task Insert(List<string> urls)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var existingUrls = entities.UrlCaches.Select(s => s.Url);
                var combinedUrls = existingUrls.Concat(urls).Distinct();
                var newUrls = await combinedUrls.Except(existingUrls).ToListAsync();
                var urlCaches = newUrls.Select(s => new UrlCache { Url = s });
                entities.UrlCaches.AddRange(urlCaches);
                await entities.SaveChangesAsync();
            }
        }
        internal async Task<List<UrlCache>> GetLatestUrls()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.UrlCaches.Where(s => s.Stamp >= yesterday).ToListAsync();
        }
        internal async Task DeleteOlderThan(DateTime oldestDate)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var tooOld = entities.UrlCaches.Where(s => s.Stamp < oldestDate);
                entities.UrlCaches.RemoveRange(tooOld);
                await entities.SaveChangesAsync();
            }
        }
    }
}
