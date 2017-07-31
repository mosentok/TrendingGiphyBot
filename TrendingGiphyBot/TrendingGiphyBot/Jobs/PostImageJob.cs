using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal List<JobConfig> JobConfigs { get; } = new List<JobConfig>();
        internal List<ulong> ChannelIds => JobConfigs?.Select(s => s.ChannelId.ToULong()).ToList();
        internal PostImageJob(IServiceProvider services, int interval, string time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected internal override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
                var jobConfigsNotInQuietHours = JobConfigs.Where(s => !s.IsInQuietHours(DateTime.Now.Hour)).ToList();
                var channelsForNewUrl = jobConfigsNotInQuietHours.Where(s => UrlHasNotBeenPostedToChannel(s.ChannelId, latestUrl)).ToList();
                await PostNewUrls(latestUrl, channelsForNewUrl);
                var channelsWithRandomStringOn = jobConfigsNotInQuietHours.Except(channelsForNewUrl).Where(s => s.RandomIsOn && !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                await PostChannelsWithRandomStringOn(channelsWithRandomStringOn);
                var channelsWithRandomStringOff = jobConfigsNotInQuietHours.Except(channelsForNewUrl).Where(s => s.RandomIsOn && string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                await PostChannelsWithRandomStringOff(channelsWithRandomStringOff);
            }
        }
        async Task PostNewUrls(string latestUrl, List<JobConfig> channelsNotPostedToYet)
        {
            foreach (var s in channelsNotPostedToYet)
                if (DiscordClient.GetChannel(s.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync($"*Trending!* {latestUrl}");
                    var history = new UrlHistory { ChannelId = s.ChannelId, Url = latestUrl };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
        }
        async Task PostChannelsWithRandomStringOn(List<JobConfig> channelsWithRandomStringOn)
        {
            foreach (var jobConfig in channelsWithRandomStringOn)
            {
                var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Tag = jobConfig.RandomSearchString, Rating = GlobalConfig.Ratings });
                await PostGif(jobConfig, giphyRandomResult.Data.Url);
            }
        }
        async Task PostChannelsWithRandomStringOff(List<JobConfig> channelsWithRandomStringOff)
        {
            var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings });
            foreach (var jobConfig in channelsWithRandomStringOff)
                await PostGif(jobConfig, giphyRandomResult.Data.Url);
        }
        async Task PostGif(JobConfig jobConfig, string url)
        {
            if (!await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, url)
                && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
            {
                await socketTextChannel.SendMessageAsync(url);
                var history = new UrlHistory { ChannelId = jobConfig.ChannelId, Url = url };
                await GlobalConfig.UrlHistoryDal.Insert(history);
            }
        }
        bool UrlHasNotBeenPostedToChannel(decimal channelId, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(channelId).Result &&
            !GlobalConfig.UrlHistoryDal.Any(channelId, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
