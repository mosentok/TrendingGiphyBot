using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal ulong ChannelId { get; private set; }
        public PostImageJob(IServiceProvider services, JobConfig jobConfig) : base(services, LogManager.GetCurrentClassLogger(), jobConfig)
        {
            ChannelId = Convert.ToUInt64(jobConfig.ChannelId);
        }
        protected override async Task Run()
        {
            if (await GlobalConfig.JobConfigDal.Any(ChannelId))
            {
                var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
                if (!string.IsNullOrEmpty(latestUrl)
                    && !await GlobalConfig.UrlHistoryDal.Any(ChannelId, latestUrl)
                    && DiscordClient.GetChannel(ChannelId) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync(latestUrl);
                    var history = new UrlHistory { ChannelId = ChannelId, Url = latestUrl };
                    await GlobalConfig.UrlHistoryDal.Insert(history);
                }
            }
        }
        protected override void TimerStartedLog() => Logger.Info($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel ID: {ChannelId}.");
    }
}
