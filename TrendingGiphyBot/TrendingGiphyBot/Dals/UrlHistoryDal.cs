using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class UrlHistoryDal
    {
        readonly string _ConnectionString;
        public UrlHistoryDal(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        internal Task<bool> Any(ulong channelId, string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.UrlHistories.Any(s => s.ChannelId == channelId && s.Url == url);
            });
        }
        internal async Task Insert(UrlHistory urlHistory)
        {
            using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
            {
                entities.UrlHistories.Add(urlHistory);
                await entities.SaveChangesAsync();
            }
        }
        internal Task<UrlHistory> Get(ulong channelId, string url)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.UrlHistories.SingleOrDefault(s => s.ChannelId == channelId && s.Url == url);
            });
        }
    }
}
