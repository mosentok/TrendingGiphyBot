using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal List<ulong> ChannelIds { get; } = new List<ulong>();
        public PostImageJob(IServiceProvider services, JobConfig jobConfig) : base(services, LogManager.GetCurrentClassLogger(), jobConfig.Interval, jobConfig.Time) { }
        protected override async Task Run()
        {
            var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
            if (!string.IsNullOrEmpty(latestUrl))
            {
                var tasks = ChannelIds.Where(s => UrlHasntBeenPostedToChannel(s, latestUrl)).Select(s =>
                {
                    if (DiscordClient.GetChannel(s) is SocketTextChannel socketTextChannel)
                    {
                        socketTextChannel.SendMessageAsync(latestUrl);
                        var history = new UrlHistory { ChannelId = s, Url = latestUrl };
                        return GlobalConfig.UrlHistoryDal.Insert(history);
                    }
                    return Task.CompletedTask;
                });
                await Task.WhenAll(tasks);
            }
        }
        bool UrlHasntBeenPostedToChannel(ulong s, string latestUrl) =>
            GlobalConfig.JobConfigDal.Any(s).Result && !GlobalConfig.UrlHistoryDal.Any(s, latestUrl).Result;
        protected override void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel IDs: {string.Join(", ", ChannelIds)}.");
    }
}
