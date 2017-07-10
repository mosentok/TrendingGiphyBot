using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using System.Linq;
using NLog;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        static Dictionary<int, string> _Images = new Dictionary<int, string>();
        internal async static Task<string> GetImageUrl()
        {
            var minute = DateTime.Now.Minute;
            while (!_Images.ContainsKey(minute))
                await Task.Delay(TimeSpan.FromSeconds(1));
            return _Images[minute];
        }
        public RefreshImagesJob(Giphy giphyClient, DiscordSocketClient discordClient, int interval, string time) : base(giphyClient, discordClient, interval, time, _Logger) { }
        protected override async Task Run()
        {
            var gifResult = await GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            _Images[DateTime.Now.Minute] = url;
        }
    }
}
