using System;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using System.Linq;
using NLog;
using TrendingGiphyBot.Enums;
using System.Net;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        public RefreshImagesJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            try
            {
                var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
                var url = gifResult.Data.FirstOrDefault()?.Url;
                if (!await GlobalConfig.UrlCacheDal.Any(url))
                    await GlobalConfig.UrlCacheDal.Insert(url);
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
