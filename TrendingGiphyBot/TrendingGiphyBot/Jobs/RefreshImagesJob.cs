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
        protected internal override async Task Run()
        {
            try
            {
                var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
                var bitlyUrl = gifResult.Data.FirstOrDefault()?.BitlyUrl;
                if (!await GlobalConfig.UrlCacheDal.Any(bitlyUrl))
                    await GlobalConfig.UrlCacheDal.Insert(bitlyUrl);
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
