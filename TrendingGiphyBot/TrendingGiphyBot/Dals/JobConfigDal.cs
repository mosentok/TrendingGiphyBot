using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Dals
{
    public class JobConfigDal : Dal
    {
        static readonly string _MinuteString = Time.Minute.ToString();
        static readonly string _MinutesString = Time.Minutes.ToString();
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
        //TODO save minutes in job config table so it doesn't have to be recalculated like this
        internal async Task<List<JobConfig>> Get(List<int> allValidMinutes)
        {
            using (var entities = new TrendingGiphyBotEntities(ConnectionString))
                return await (from jobConfig in entities.JobConfigs
                              let isMinutes = jobConfig.Time == _MinuteString || jobConfig.Time == _MinutesString
                              let isHours = jobConfig.Time == _HourString || jobConfig.Time == _HoursString
                              let intervalMinutes = isMinutes ? jobConfig.Interval :
                                                        isHours ? jobConfig.Interval * 60 : -1
                              where allValidMinutes.Contains(intervalMinutes)
                              select jobConfig).ToListAsync();
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
    }
}
