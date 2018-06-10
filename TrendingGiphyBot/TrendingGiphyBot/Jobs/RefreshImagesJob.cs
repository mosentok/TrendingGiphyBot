using System.Linq;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        internal RefreshImagesJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            await Logger.SwallowAsync(async () =>
            {
                var trendingParameter = new TrendingParameter { Rating = GlobalConfig.Ratings };
                var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(trendingParameter);
                var urls = await gifResult.Data.Select(s => s.Url).Except(GlobalConfig.Config.UrlsToIgnore).WhereAsync(async s => !await GlobalConfig.UrlCacheDal.Any(s));
                await GlobalConfig.UrlCacheDal.Insert(urls);
                await GlobalConfig.UpdateLatestUrls();
            });
        }
    }
}
