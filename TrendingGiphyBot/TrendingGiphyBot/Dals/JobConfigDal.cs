using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Dals
{
    public class JobConfigDal : Dal
    {
        internal JobConfigDal(string connectionString) : base(connectionString) { }
        internal Task<JobConfig> Get(decimal id)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.JobConfigs.SingleOrDefault(s => s.ChannelId == id);
            });
        }
        internal Task<bool> Any(decimal id)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.JobConfigs.Any(s => s.ChannelId == id);
            });
        }
        internal Task<List<JobConfig>> Get(int interval, Time time)
        {
            return Task.Run(() =>
            {
                using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                    return entities.JobConfigs.Where(s => s.Interval == interval && s.Time == time.ToString()).ToList();
            });
        }
        internal async Task Insert(JobConfig config)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                entities.JobConfigs.Add(config);
                await entities.SaveChangesAsync();
            }
        }
        internal async Task Update(JobConfig config)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == config.ChannelId);
                match.Interval = config.Interval;
                match.Time = config.Time;
                await entities.SaveChangesAsync();
            }
        }
        internal async Task UpdateRandom(JobConfig config)
        {
            //TODO centralize random set
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == config.ChannelId);
                match.RandomIsOn = config.RandomIsOn;
                match.RandomSearchString = config.RandomSearchString;
                await entities.SaveChangesAsync();
            }
        }
        internal async Task UpdateQuietHours(JobConfig config)
        {
            //TODO centralize min/max quiet hours set
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == config.ChannelId);
                match.MinQuietHour = config.MinQuietHour;
                match.MaxQuietHour = config.MaxQuietHour;
                await entities.SaveChangesAsync();
            }
        }
        internal async Task Remove(decimal channelId)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == channelId);
                entities.JobConfigs.Remove(match);
                await entities.SaveChangesAsync();
            }
        }
    }
}
