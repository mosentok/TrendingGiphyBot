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
        public RefreshImagesJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected internal override async Task Run()
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
            var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(new TrendingParameter());
            foreach (var data in gifResult.Data)
                if (!await GlobalConfig.UrlCacheDal.Any(data.Url))
                    return data;
            return null;
        }
    }
}
