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
                        socketTextChannel.SendMessageAsync(latestUrl);
                        var history = new UrlHistory { ChannelId = s.ChannelId, Url = latestUrl };
                        return GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                    return Task.CompletedTask;
                }).ToList();
                await Task.WhenAll(tasks);
                var channelsStillNotPostedToYet = JobConfigs.Except(channelsNotPostedToYet);
                var channelsWithRandomOn = channelsStillNotPostedToYet.Where(s => s.RandomIsOn);
                var randomTasks = channelsWithRandomOn.Select(s =>
                {
                    if (DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) is SocketTextChannel socketTextChannel)
                    {
                        var url = GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Tag = s.RandomSearchString }).Result.Data.Url;
                        return socketTextChannel.SendMessageAsync(url);
                    }
                    return Task.CompletedTask;
                });
                await Task.WhenAll(randomTasks);
            }
        }
        bool UrlHasNotBeenPostedToChannel(ulong channelId, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(channelId).Result &&
            !GlobalConfig.UrlHistoryDal.Any(channelId, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
