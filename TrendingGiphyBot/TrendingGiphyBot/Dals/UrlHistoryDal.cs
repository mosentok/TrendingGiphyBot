using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public class UrlHistoryDal : Dal
    {
        internal UrlHistoryDal(string connectionString) : base(connectionString) { }
        internal async Task<bool> Any(decimal channelId, string url)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.UrlHistories.AnyAsync(s => s.ChannelId == channelId && s.Url == url);
        }
        internal async Task Insert(List<UrlHistory> successes)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                entities.UrlHistories.AddRange(successes);
                await entities.SaveChangesAsync();
            }
        }
        internal async Task DeleteOlderThan(DateTime oldestDate)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var tooOld = entities.UrlHistories.Where(s => s.Stamp < oldestDate);
                entities.UrlHistories.RemoveRange(tooOld);
                await entities.SaveChangesAsync();
            }
        }
    }
}
