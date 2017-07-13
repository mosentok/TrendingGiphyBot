using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        internal ulong ChannelId { get; private set; }
        string _LastUrlIPosted;
        internal async Task<string> GetImageUrl()
        {
            var minute = Convert.ToInt16(DateTime.Now.Minute);
            while (!await GlobalConfig.UrlCacheDal.Any(minute))
                await Task.Delay(TimeSpan.FromSeconds(1));
            return (await GlobalConfig.UrlCacheDal.Get(minute)).Url;
        }
        public PostImageJob(IServiceProvider services, DiscordSocketClient discordClient, JobConfig jobConfig) : base(services, discordClient, jobConfig, _Logger)
        {
            ChannelId = Convert.ToUInt64(jobConfig.ChannelId);
        }
        protected override async Task Run()
        {
            if (await GlobalConfig.JobConfigDal.Any(ChannelId))
            {
                var url = await GetImageUrl();
                if (!string.IsNullOrEmpty(url)
                    && url != _LastUrlIPosted
                    && DiscordClient.GetChannel(ChannelId) is SocketTextChannel socketTextChannel)
                {
                    await socketTextChannel.SendMessageAsync(url);
                    _LastUrlIPosted = url;
                }
            }
        }
        protected override void TimerStartedLog() => _Logger.Info($"Config: {Interval} {Time}. Next elapse: {NextElapse}. Channel ID: {ChannelId}.");
    }
}
