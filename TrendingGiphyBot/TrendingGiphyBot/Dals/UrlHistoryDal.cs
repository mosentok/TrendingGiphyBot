using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class UrlHistoryDal : Dal
    {
        public UrlHistoryDal(string connectionString) : base(connectionString) { }
        internal Task<bool> Any(decimal channelId, string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlHistories.Any(s => s.ChannelId == channelId && s.Url == url);
            });
        }
        internal async Task Insert(UrlHistory urlHistory)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                entities.UrlHistories.Add(urlHistory);
                await entities.SaveChangesAsync();
            }
        }
        internal Task<UrlHistory> Get(decimal channelId, string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.UrlHistories.SingleOrDefault(s => s.ChannelId == channelId && s.Url == url);
            });
        }
    }
}
