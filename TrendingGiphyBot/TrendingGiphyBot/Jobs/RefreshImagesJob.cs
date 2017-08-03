using System;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
using TrendingGiphyBot.Enums;
using System.Net;
using GiphyDotNet.Model.GiphyImage;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        internal RefreshImagesJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            try
            {
                var newTrendingGif = await FindNewTrendingGif();
                if (newTrendingGif != null)
                    await GlobalConfig.UrlCacheDal.Insert(newTrendingGif.Url);
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
            }
        }
        async Task<Data> FindNewTrendingGif()
        {
            var trendingParameter = new TrendingParameter { Rating = GlobalConfig.Ratings };
            var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(trendingParameter);
            foreach (var data in gifResult.Data)
                if (!await GlobalConfig.UrlCacheDal.Any(data.Url))
                    return data;
            return null;
        }
    }
}
