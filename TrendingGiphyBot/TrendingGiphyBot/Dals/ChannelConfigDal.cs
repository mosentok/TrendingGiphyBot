using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public class ChannelConfigDal : Dal
    {
        internal ChannelConfigDal(string connectionString) : base(connectionString) { }
        internal async Task<bool> Any(ulong channelId)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.ChannelConfigs.AnyAsync(s => s.ChannelId == channelId);
        }
        internal async Task<string> GetPrefix(ulong channelId)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return (await entities.ChannelConfigs.SingleAsync(s => s.ChannelId == channelId)).Prefix;
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
