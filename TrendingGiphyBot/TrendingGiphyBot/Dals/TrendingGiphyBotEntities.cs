using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Dals
{
    public partial class TrendingGiphyBotEntities
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        static readonly string _HourString = Time.Hour.ToString();
        static readonly string _HoursString = Time.Hours.ToString();
        public TrendingGiphyBotEntities(string nameOrConnectionString) : base(nameOrConnectionString) { }
        internal async Task<bool> AnyChannelConfigs(ulong channelId)
        {
            return await ChannelConfigs.AnyAsync(s => s.ChannelId == channelId);
        }
        internal async Task<string> GetPrefix(ulong channelId)
        {
            return await ChannelConfigs.Where(s => s.ChannelId == channelId).Select(s => s.Prefix).SingleOrDefaultAsync();
        }
        internal async Task SetPrefix(ulong channelId, string prefix)
        {
            var config = await ChannelConfigs.SingleAsync(s => s.ChannelId == channelId);
            config.Prefix = prefix;
            await SaveChangesAsync();
        }
        internal async Task InsertChannelConfig(ulong channelId, string prefix)
        {
            var config = new ChannelConfig { ChannelId = channelId, Prefix = prefix };
            ChannelConfigs.Add(config);
            await SaveChangesAsync();
        }
        internal async Task<JobConfig> GetJobConfig(decimal id)
        {
            return await JobConfigs.SingleOrDefaultAsync(s => s.ChannelId == id);
        }
        internal async Task<JobConfig> GetJobConfigWithHourOffset(decimal id, short hourOffset)
        {
            var match = await JobConfigs.SingleOrDefaultAsync(s => s.ChannelId == id);
            if (match != null)
            {
                if (match.MinQuietHour.HasValue)
                    match.MinQuietHour = UndoHourOffset(match.MinQuietHour.Value, hourOffset);
                if (match.MaxQuietHour.HasValue)
                    match.MaxQuietHour = UndoHourOffset(match.MaxQuietHour.Value, hourOffset);
            }
            return match;
        }
        internal async Task<bool> AnyJobConfig(decimal id)
        {
            return await JobConfigs.AnyAsync(s => s.ChannelId == id);
        }
        internal async Task<List<decimal>> FindMatchingIds(IEnumerable<decimal> ids)
        {
            var jobConfigIds = JobConfigs.Select(s => s.ChannelId);
            return await jobConfigIds.Intersect(ids).ToListAsync();
        }
        internal async Task<List<JobConfig>> GetJobConfigsToRun(List<int> curentValidMinutes)
        {
            return await (from jobConfig in JobConfigs
                          where curentValidMinutes.Contains(jobConfig.IntervalMinutes)
                          select jobConfig).ToListAsync();
        }
        internal async Task InsertJobConfig(JobConfig config)
        {
            config.IntervalMinutes = DetermineIntervalMinutes(config);
            JobConfigs.Add(config);
            await SaveChangesAsync();
        }
        internal async Task UpdateJobConfig(JobConfig config)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == config.ChannelId);
            match.Interval = config.Interval;
            match.Time = config.Time;
            match.IntervalMinutes = DetermineIntervalMinutes(config);
            await SaveChangesAsync();
        }
        internal async Task UpdateInterval(ulong channelId, int interval, Time time)
        {
            var timeString = time.ToString();
            var intervalMinutes = DetermineIntervalMinutes(interval, timeString);
            var any = await AnyJobConfig(channelId);
            if (any)
            {
                var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
                match.Interval = interval;
                match.Time = timeString;
                match.IntervalMinutes = intervalMinutes;
            }
            else
            {
                var config = new JobConfig
                {
                    ChannelId = channelId,
                    Interval = interval,
                    Time = timeString,
                    IntervalMinutes = intervalMinutes
                };
                JobConfigs.Add(config);
            }
            await SaveChangesAsync();
        }
        static int DetermineIntervalMinutes(JobConfig config)
        {
            if (config.Time == _HourString || config.Time == _HoursString)
                return config.Interval * 60;
            return config.Interval;
        }
        static int DetermineIntervalMinutes(int interval, string timeString)
        {
            if (timeString == _HourString || timeString == _HoursString)
                return interval * 60;
            return interval;
        }
        internal async Task UpdateRandom(JobConfig config)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == config.ChannelId);
            match.RandomIsOn = config.RandomIsOn;
            match.RandomSearchString = config.RandomSearchString;
            await SaveChangesAsync();
        }
        internal async Task UpdateRandom(ulong channelId, bool randomIsOn, string randomSearchString)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = randomIsOn;
            match.RandomSearchString = randomSearchString;
            await SaveChangesAsync();
        }
        internal async Task UpdateQuietHours(JobConfig config)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == config.ChannelId);
            match.MinQuietHour = config.MinQuietHour;
            match.MaxQuietHour = config.MaxQuietHour;
            await SaveChangesAsync();
        }
        internal async Task UpdateQuietHoursWithHourOffset(ulong channelId, short minHour, short maxHour, short hourOffset)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.MinQuietHour = ApplyHourOffset(minHour, hourOffset);
            match.MaxQuietHour = ApplyHourOffset(maxHour, hourOffset);
            await SaveChangesAsync();
        }
        internal async Task TurnOffRandom(ulong channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = false;
            match.RandomSearchString = null;
            await SaveChangesAsync();
        }
        internal async Task TurnOffQuietHours(ulong channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.MinQuietHour = null;
            match.MaxQuietHour = null;
            await SaveChangesAsync();
        }
        internal async Task RemoveJobConfig(decimal channelId)
        {
            var matches = JobConfigs.Where(s => s.ChannelId == channelId);
            JobConfigs.RemoveRange(matches);
            await SaveChangesAsync();
        }
        public async Task BlankRandomConfig(decimal channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = false;
            match.RandomSearchString = null;
            await SaveChangesAsync();
        }
        internal async Task InsertUrlCaches(List<string> urls)
        {
            var existingUrls = UrlCaches.Select(s => s.Url);
            var combinedUrls = existingUrls.Concat(urls).Distinct();
            var newUrls = await combinedUrls.Except(existingUrls).ToListAsync();
            var urlCaches = newUrls.Select(s => new UrlCache { Url = s });
            UrlCaches.AddRange(urlCaches);
            await SaveChangesAsync();
        }
        internal async Task<List<UrlCache>> GetLatestUrls()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            return await UrlCaches.Where(s => s.Stamp >= yesterday).ToListAsync();
        }
        internal async Task DeleteUrlCachesOlderThan(DateTime oldestDate)
        {
            var tooOld = UrlCaches.Where(s => s.Stamp < oldestDate);
            UrlCaches.RemoveRange(tooOld);
            await SaveChangesAsync();
        }
        internal async Task<string> GetLastestUrlNotPosted(decimal channelId)
        {
            var channelsUrls = UrlHistories.Where(s => s.ChannelId == channelId).Select(s => s.Url);
            var latestUrls = UrlCaches.Select(s => s.Url);
            return await latestUrls.Except(channelsUrls).FirstOrDefaultAsync();
        }
        internal async Task<bool> AnyUrlHistories(decimal channelId, string url)
        {
            return await UrlHistories.AnyAsync(s => s.ChannelId == channelId && s.Url == url);
        }
        internal async Task InsertUrlHistories(List<UrlHistory> histories)
        {
            UrlHistories.AddRange(histories);
            await SaveChangesAsync();
        }
        internal async Task DeleteUrlHistoriesOlderThan(DateTime oldestDate)
        {
            var tooOld = UrlHistories.Where(s => s.Stamp < oldestDate);
            UrlHistories.RemoveRange(tooOld);
            await SaveChangesAsync();
        }
        static short ApplyHourOffset(short hour, short hourOffset) => (short)((hour + hourOffset) % 24);
        static short UndoHourOffset(short hour, short hourOffset) => (short)((hour - hourOffset) % 24);
    }
}
