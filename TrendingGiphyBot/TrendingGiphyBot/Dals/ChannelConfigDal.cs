using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class ChannelConfigDal : Dal
    {
        internal ChannelConfigDal(string connectionString) : base(connectionString) { }
        internal Task<bool> Any(ulong channelId)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.ChannelConfigs.Any(s => s.ChannelId == channelId);
            });
        }
        internal Task<string> GetPrefix(ulong channelId)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.ChannelConfigs.Single(s => s.ChannelId == channelId).Prefix;
            });
        }
        internal async Task SetPrefix(ulong channelId, string prefix)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var config = entities.ChannelConfigs.Single(s => s.ChannelId == channelId);
                config.Prefix = prefix;
                await entities.SaveChangesAsync();
            }
        }
        internal async Task Insert(ulong channelId, string prefix)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var config = new ChannelConfig { ChannelId = channelId, Prefix = prefix };
                entities.ChannelConfigs.Add(config);
                await entities.SaveChangesAsync();
            }
        }
    }
}
