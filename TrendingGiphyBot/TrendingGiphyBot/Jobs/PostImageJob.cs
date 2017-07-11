using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using TrendingGiphyBot.Dals;
using NLog;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        internal ulong ChannelId { get; private set; }
        readonly JobConfigDal _JobConfigDal;
        readonly UrlCacheDal _UrlCacheDal;
        string _LastUrlIPosted;
        internal async Task<string> GetImageUrl()
        {
            var minute = Convert.ToInt16(DateTime.Now.Minute);
            while (!await _UrlCacheDal.Any(minute))
                await Task.Delay(TimeSpan.FromSeconds(1));
            return (await _UrlCacheDal.Get(minute)).Url;
        }
        public PostImageJob(Giphy giphyClient, DiscordSocketClient discordClient, JobConfig jobConfig, JobConfigDal jobConfigDal, UrlCacheDal urlCacheDal) : base(giphyClient, discordClient, jobConfig, _Logger)
        {
            ChannelId = Convert.ToUInt64(jobConfig.ChannelId);
            _UrlCacheDal = urlCacheDal;
            _JobConfigDal = jobConfigDal;
        }
        protected override async Task Run()
        {
            if (await _JobConfigDal.Any(ChannelId))
            {
                var url = await GetImageUrl();
                if (!string.IsNullOrEmpty(url)
                    && url != _LastUrlIPosted
                    && DiscordClient.GetChannel(ChannelId) is SocketTextChannel socketTextChannel)
                    await socketTextChannel.SendMessageAsync(url);
                _LastUrlIPosted = url;
            }
        }
    }
}
