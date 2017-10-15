using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
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
                var urls = await FindNewTrendingGifs();
                foreach (var url in urls)
                    await GlobalConfig.UrlCacheDal.Insert(url);
                GlobalConfig.LatestUrls = await GlobalConfig.UrlCacheDal.GetLatestUrls();
            });
        }
        async Task<List<string>> FindNewTrendingGifs()
        {
            var trendingParameter = new TrendingParameter { Rating = GlobalConfig.Ratings };
            var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(trendingParameter);
            return await gifResult.Data.Select(s => s.Url).Except(GlobalConfig.Config.UrlsToIgnore).WhereAsync(async s => !await GlobalConfig.UrlCacheDal.Any(s));
        }
    }
}
