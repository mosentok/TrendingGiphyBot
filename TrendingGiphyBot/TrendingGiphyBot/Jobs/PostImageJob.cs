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
        public PostImageJob(IServiceProvider services, JobConfig jobConfig) : base(services, LogManager.GetCurrentClassLogger(), jobConfig.Interval, jobConfig.Time) { }
        protected internal override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
                var nowHour = DateTime.Now.Hour;
                var jobConfigsNotInQuietHours = JobConfigs.Where(s => !s.IsInQuietHours(nowHour)).ToList();
                var channelsNotPostedToYet = jobConfigsNotInQuietHours.Where(s => UrlHasNotBeenPostedToChannel(s.ChannelId, latestUrl)).ToList();
                await PostNewUrls(latestUrl, channelsNotPostedToYet);
                var channelsStillNotPostedToYet = jobConfigsNotInQuietHours.Except(channelsNotPostedToYet).ToList();
                var channelsWithRandomStringOn = channelsStillNotPostedToYet.Where(s => s.RandomIsOn && !string.IsNullOrWhiteSpace(s.RandomSearchString));
                await PostRandomsWithSearchString(channelsWithRandomStringOn);
                var channelsLeftover = jobConfigsNotInQuietHours
                    .Except(channelsNotPostedToYet)
                    .Except(channelsStillNotPostedToYet)
                    .Where(s => s.RandomIsOn);
                var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter());
                var randomUrl = giphyRandomResult.Data.Url;
                await PostChannelsLeftover(channelsLeftover, randomUrl);
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
        async Task PostChannelsLeftover(IEnumerable<JobConfig> channelsLeftover, string randomUrl)
        {
            foreach (var s in channelsLeftover)
                if (!await GlobalConfig.UrlHistoryDal.Any(s.ChannelId, randomUrl)
                    && DiscordClient.GetChannel(s.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync(randomUrl);
                    var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrl };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
        }
        async Task PostRandomsWithSearchString(IEnumerable<JobConfig> channelsWithRandomStringOn)
        {
            foreach (var s in channelsWithRandomStringOn)
                if (DiscordClient.GetChannel(s.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Tag = s.RandomSearchString });
                    if (!await GlobalConfig.UrlHistoryDal.Any(s.ChannelId, giphyRandomResult.Data.Url))
                    {
                        var randomUrlWithString = giphyRandomResult.Data.Url;
                        await socketTextChannel.SendMessageAsync(randomUrlWithString);
                        var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrlWithString };
                        await GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                }
        }
        bool UrlHasNotBeenPostedToChannel(decimal channelId, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(channelId).Result &&
            !GlobalConfig.UrlHistoryDal.Any(channelId, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
