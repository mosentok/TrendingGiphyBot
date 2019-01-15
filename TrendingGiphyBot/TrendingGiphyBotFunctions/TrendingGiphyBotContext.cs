using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TrendingGiphyBotFunctions
{
    public class TrendingGiphyBotContext : DbContext
    {
        readonly string _ConnectionString;
        readonly int _CommandTimeout;
        protected TrendingGiphyBotContext() { }
        public TrendingGiphyBotContext(DbContextOptions options) : base(options) { }
        public TrendingGiphyBotContext(string connectionString, int commandTimeout)
        {
            _ConnectionString = connectionString;
            _CommandTimeout = commandTimeout;
        }
        public DbSet<UrlCache> UrlCaches { get; set; }
        public DbSet<UrlHistory> UrlHistories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_ConnectionString, s => s.CommandTimeout(_CommandTimeout));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlHistory>().HasKey(s => new { s.ChannelId, s.Url });
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();
        }
        public async Task<int> DeleteUrlCachesOlderThan(DateTime oldestDate)
        {
            var sql = "DELETE FROM UrlCache WHERE Stamp < @oldestDate";
            var parameter = new SqlParameter("@oldestDate", oldestDate);
            return await Database.ExecuteSqlCommandAsync(sql, parameter);
        }
        public async Task<int> DeleteUrlHistoriesOlderThan(DateTime oldestDate)
        {
            var sql = "DELETE FROM UrlHistory WHERE Stamp < @oldestDate";
            var parameter = new SqlParameter("@oldestDate", oldestDate);
            return await Database.ExecuteSqlCommandAsync(sql, parameter);
        }
        public async Task<int> InsertNewTrendingGifs(List<GifObject> gifObjects)
        {
            var existingGifObjects = from gifObject in gifObjects
                                     join urlCache in UrlCaches on gifObject.Url equals urlCache.Url
                                     select gifObject;
            var newGifObjects = gifObjects.Except(existingGifObjects);
            var newUrlCaches = newGifObjects.Select(s => new UrlCache { Url = s.Url, Stamp = DateTime.Now });
            await UrlCaches.AddRangeAsync(newUrlCaches);
            return await SaveChangesAsync();
        }
    }
}
