using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    class JobConfigDal
    {
        readonly string _ConnectionString;
        public JobConfigDal(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        internal Task<int> GetCount()
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.JobConfigs.Count();
            });
        }
        internal Task<List<JobConfig>> GetAll()
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.JobConfigs.ToList();
            });
        }
        internal Task<JobConfig> Get(ulong id)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.JobConfigs.SingleOrDefault(s => s.ChannelId == id);
            });
        }
        internal Task<bool> Any(ulong id)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
                    return entities.JobConfigs.Any(s => s.ChannelId == id);
            });
        }
        internal async Task Insert(JobConfig config)
        {
            using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
            {
                entities.JobConfigs.Add(config);
                await entities.SaveChangesAsync();
            }
        }
        internal async Task Update(JobConfig config)
        {
            using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == config.ChannelId);
                match.Interval = config.Interval;
                match.Time = config.Time.ToString();
                await entities.SaveChangesAsync();
            }
        }
        internal async Task Remove(ulong id)
        {
            using (var entities = new TrendingGiphyBotEntities(_ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == id);
                entities.JobConfigs.Remove(match);
                await entities.SaveChangesAsync();
            }
        }
    }
}
