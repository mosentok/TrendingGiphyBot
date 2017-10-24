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
        internal async Task<bool> Any(string url)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.UrlCaches.AnyAsync(s => s.Url == url);
        }
        internal async Task<bool> Any()
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.UrlCaches.AnyAsync();
        }
        internal async Task Insert(string url)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                entities.UrlCaches.Add(new UrlCache { Url = url });
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
