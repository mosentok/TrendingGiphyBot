using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using System.Linq;
using NLog;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        readonly UrlCacheDal _UrlCacheDal;
        public RefreshImagesJob(Giphy giphyClient, DiscordSocketClient discordClient, int interval, string time, UrlCacheDal urlCacheDal) : base(giphyClient, discordClient, interval, time, _Logger)
        {
            _UrlCacheDal = urlCacheDal;
        }
        protected override async Task Run()
        {
            var gifResult = await GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            var minute = Convert.ToInt16(DateTime.Now.Minute);
            var urlCache = new UrlCache { Minute = minute, Url = url };
            if (await _UrlCacheDal.Any(minute))
                await _UrlCacheDal.Update(urlCache);
            else
                await _UrlCacheDal.Insert(urlCache);
        }
    }
}
