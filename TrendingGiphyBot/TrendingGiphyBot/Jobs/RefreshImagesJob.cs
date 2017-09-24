using System.Collections.Generic;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
using GiphyDotNet.Model.GiphyImage;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        internal RefreshImagesJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            await Logger.SwallowAsync(async () =>
            {
                var results = await FindNewTrendingGifs();
                results.ForEach(async s => await GlobalConfig.UrlCacheDal.Insert(s.Url));
            });
        }
        async Task<List<Data>> FindNewTrendingGifs()
        {
            var trendingParameter = new TrendingParameter { Rating = GlobalConfig.Ratings };
            var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(trendingParameter);
            return await gifResult.Data.WhereAsync(async s => !await GlobalConfig.UrlCacheDal.Any(s.Url));
        }
    }
}
