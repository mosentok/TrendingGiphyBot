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
            var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
            if (!string.IsNullOrEmpty(latestUrl))
            {
                var channelsNotPostedToYet = JobConfigs.Where(s => UrlHasNotBeenPostedToChannel(Convert.ToUInt64(s.ChannelId), latestUrl)).ToList();
                var tasks = channelsNotPostedToYet.Select(s =>
                {
                    if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                    {
                        socketTextChannel.SendMessageAsync($"*Trending!* {latestUrl}");
                        var history = new UrlHistory { ChannelId = s.ChannelId, Url = latestUrl };
                        return GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                    return Task.CompletedTask;
                }).ToList();
                await Task.WhenAll(tasks);
                var channelsStillNotPostedToYet = JobConfigs.Except(channelsNotPostedToYet).ToList();
                var channelsWithRandomStringOn = channelsStillNotPostedToYet.Where(s => s.RandomIsOn && !string.IsNullOrWhiteSpace(s.RandomSearchString));
                var randomWithStringTasks = channelsWithRandomStringOn.Select(s =>
                {
                    if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                    {
                        var randomUrlWithString = GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Tag = s.RandomSearchString }).Result.Data.Url;
                        socketTextChannel.SendMessageAsync(randomUrlWithString).Wait();
                        var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrlWithString };
                        return GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                    return Task.CompletedTask;
                });
                await Task.WhenAll(randomWithStringTasks);
                var channelsLeftover = JobConfigs
                    .Except(channelsNotPostedToYet)
                    .Except(channelsStillNotPostedToYet);
                var randomUrl = (await GlobalConfig.GiphyClient.RandomGif(new RandomParameter())).Data.Url;
                var leftoverTasks = channelsLeftover.Select(s =>
                {
                    if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                    {
                        socketTextChannel.SendMessageAsync(randomUrl).Wait();
                        var history = new UrlHistory { ChannelId = s.ChannelId, Url = randomUrl };
                        return GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                    return Task.CompletedTask;
                });
                await Task.WhenAll(leftoverTasks);
            }
        }
        bool UrlHasNotBeenPostedToChannel(ulong channelId, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(channelId).Result &&
            !GlobalConfig.UrlHistoryDal.Any(channelId, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
