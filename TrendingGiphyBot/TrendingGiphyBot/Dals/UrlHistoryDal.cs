using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public class UrlHistoryDal : Dal
    {
        internal UrlHistoryDal(string connectionString) : base(connectionString) { }
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
    }
}
