using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using GiphyDotNet.Model.Parameters;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal List<JobConfig> JobConfigs { get; } = new List<JobConfig>();
        internal List<ulong> ChannelIds => JobConfigs?.Select(s => Convert.ToUInt64(s.ChannelId)).ToList();
        public PostImageJob(IServiceProvider services, JobConfig jobConfig) : base(services, LogManager.GetCurrentClassLogger(), jobConfig.Interval, jobConfig.Time) { }
        protected internal override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
                if (!string.IsNullOrEmpty(latestUrl))
                {
                    var channelsNotPostedToYet = JobConfigs.Where(s => UrlHasNotBeenPostedToChannel(Convert.ToUInt64(s.ChannelId), latestUrl)).ToList();
                    await PostNewUrls(latestUrl, channelsNotPostedToYet);
                    var channelsStillNotPostedToYet = JobConfigs.Except(channelsNotPostedToYet).ToList();
                    var channelsWithRandomStringOn = channelsStillNotPostedToYet.Where(s => s.RandomIsOn && !string.IsNullOrWhiteSpace(s.RandomSearchString));
                    await PostRandomsWithSearchString(channelsWithRandomStringOn);
                    var channelsLeftover = JobConfigs
                        .Except(channelsNotPostedToYet)
                        .Except(channelsStillNotPostedToYet)
                        .Where(s => s.RandomIsOn);
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter());
                    var randomUrl = giphyRandomResult.Data.Url;
                    await PostChannelsLeftover(channelsLeftover, randomUrl);
                }
            }
        }
        async Task PostNewUrls(string latestUrl, List<JobConfig> channelsNotPostedToYet)
        {
            foreach (var s in channelsNotPostedToYet)
                if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync($"*Trending!* {latestUrl}");
                    var history = new UrlHistory { ChannelId = s.ChannelId, Url = latestUrl };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
        }
        async Task PostChannelsLeftover(IEnumerable<JobConfig> channelsLeftover, string randomUrl)
        {
            foreach (var s in channelsLeftover)
                if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync(randomUrl);
                    var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrl };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
        }
        async Task PostRandomsWithSearchString(IEnumerable<JobConfig> channelsWithRandomStringOn)
        {
            foreach (var s in channelsWithRandomStringOn)
                if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Tag = s.RandomSearchString });
                    var randomUrlWithString = giphyRandomResult.Data.Url;
                    await socketTextChannel.SendMessageAsync(randomUrlWithString);
                    var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrlWithString };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
        }
        bool UrlHasNotBeenPostedToChannel(ulong channelId, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(channelId).Result &&
            !GlobalConfig.UrlHistoryDal.Any(channelId, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
