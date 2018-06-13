using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Dals
{
    public class JobConfigDal : Dal
    {
        static readonly string _HourString = Time.Hour.ToString();
        static readonly string _HoursString = Time.Hours.ToString();
        internal JobConfigDal(string connectionString) : base(connectionString) { }
        internal async Task<JobConfig> Get(decimal id)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.JobConfigs.SingleOrDefaultAsync(s => s.ChannelId == id);
        }
        internal async Task<bool> Any(decimal id)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await entities.JobConfigs.AnyAsync(s => s.ChannelId == id);
        }
        internal async Task<List<JobConfig>> Get(List<int> curentValidMinutes)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await (from jobConfig in entities.JobConfigs
                              where curentValidMinutes.Contains(jobConfig.IntervalMinutes)
                              select jobConfig).ToListAsync();
        }
        internal async Task Insert(JobConfig config)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                config.IntervalMinutes = DetermineIntervalMinutes(config);
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
                match.IntervalMinutes = DetermineIntervalMinutes(config);
                await entities.SaveChangesAsync();
            }
        }
        static int DetermineIntervalMinutes(JobConfig config)
        {
            if (config.Time == _HourString || config.Time == _HoursString)
                return config.Interval * 60;
            return config.Interval;
        }
        internal async Task UpdateRandom(JobConfig config)
        {
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
        public async Task BlankRandomConfig(decimal channelId)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
            {
                var match = entities.JobConfigs.Single(s => s.ChannelId == channelId);
                match.RandomIsOn = false;
                match.RandomSearchString = null;
                await entities.SaveChangesAsync();
            }
        }
    }
}
