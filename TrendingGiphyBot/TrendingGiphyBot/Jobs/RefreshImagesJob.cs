using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
using TrendingGiphyBot.Enums;
using GiphyDotNet.Model.GiphyImage;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        internal RefreshImagesJob(IGlobalConfig globalConfig, int interval, Time time) : base(globalConfig, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            await Logger.SwallowAsync(async () =>
            {
                var newTrendingGif = await FindNewTrendingGif();
                if (newTrendingGif != null)
                    await GlobalConfig.UrlCacheDal.Insert(newTrendingGif.Url);
            });
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
