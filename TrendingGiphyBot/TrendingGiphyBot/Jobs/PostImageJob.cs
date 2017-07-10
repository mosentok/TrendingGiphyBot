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
        readonly ulong _ChannelId;
        readonly JobConfigDal _JobConfigDal;
        string _LastUrlIPosted;
        public PostImageJob(Giphy giphyClient, DiscordSocketClient discordClient, JobConfig jobConfig, JobConfigDal jobConfigDal) : base(giphyClient, discordClient, jobConfig, _Logger)
        {
            _ChannelId = Convert.ToUInt64(jobConfig.ChannelId);
            _JobConfigDal = jobConfigDal;
        }
        protected override async Task Run()
        {
            if (await _JobConfigDal.Any(_ChannelId))
            {
                var url = await RefreshImagesJob.GetImageUrl();
                if (!string.IsNullOrEmpty(url) && url != _LastUrlIPosted)
                {
                    var socketTextChannel = DiscordClient.GetChannel(_ChannelId) as SocketTextChannel;
                    if (socketTextChannel != null)
                        await socketTextChannel.SendMessageAsync(url);
                }
                _LastUrlIPosted = url;
            }
        }
    }
}
