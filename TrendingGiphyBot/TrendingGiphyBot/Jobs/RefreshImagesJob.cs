using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Model.Parameters;
using System.Linq;
using NLog;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        public RefreshImagesJob(IServiceProvider services, int interval, string time) : base(services, interval, time, _Logger) { }
        protected override async Task Run()
        {
            var gifResult = await  GlobalConfig.GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            var minute = Convert.ToInt16(DateTime.Now.Minute);
            var urlCache = new UrlCache { Minute = minute, Url = url };
            if (await GlobalConfig.UrlCacheDal.Any(minute))
                await GlobalConfig.UrlCacheDal.Update(urlCache);
            else
                await GlobalConfig.UrlCacheDal.Insert(urlCache);
        }
    }
}
