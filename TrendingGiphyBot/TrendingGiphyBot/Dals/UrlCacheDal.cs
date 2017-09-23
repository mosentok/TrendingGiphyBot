using System;
using System.Collections.Generic;
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
        internal Task<List<string>> GetLatestUrls()
        {
            return Task.Run(() =>
            {
                var yesterday = DateTime.Now.AddDays(-1);
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlCaches.Where(s => s.Stamp >= yesterday).Select(s => s.Url).ToList();
            });
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
