using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        readonly ulong _ChannelId;
        string _LastUrlIPosted;
        public PostImageJob(Giphy giphyClient, DiscordSocketClient discordClient, JobConfig jobConfig) : base(giphyClient, discordClient, jobConfig)
        {
            _ChannelId = Convert.ToUInt64(jobConfig.ChannelId);
        }
        protected override async Task Run()
        {
            var fireTime = DateTime.Now;
            Console.WriteLine($"{nameof(fireTime)}:{fireTime.ToString("o")}");
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
