using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Dals
{
    public class UrlHistoryDal : Dal
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        internal UrlHistoryDal(string connectionString) : base(connectionString) { }
        internal async Task<bool> Any(decimal channelId, string url)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.UrlHistories.AnyAsync(s => s.ChannelId == channelId && s.Url == url);
        }
        internal async Task Insert(List<UrlHistory> histories)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                foreach (var history in histories)
                    await _Logger.SwallowAsync(async () =>
                    {
                        entities.UrlHistories.Add(history);
                        await entities.SaveChangesAsync();
                    });
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
