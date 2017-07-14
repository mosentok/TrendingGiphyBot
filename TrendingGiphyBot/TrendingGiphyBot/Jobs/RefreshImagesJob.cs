using System;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using System.Linq;
using NLog;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        public RefreshImagesJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            var gifResult = await  GlobalConfig.GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            if (!await GlobalConfig.UrlCacheDal.Any(url))
            {
                var urlCache = new UrlCache { Url = url, Stamp = DateTime.Now };
                await GlobalConfig.UrlCacheDal.Insert(urlCache);
            }
        }
    }
}
