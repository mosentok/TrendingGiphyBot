using System.Linq;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using NLog;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        internal RefreshImagesJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            var trendingParameter = new TrendingParameter { Rating = GlobalConfig.Ratings };
            var gifResult = await GlobalConfig.GiphyClient.TrendingGifs(trendingParameter);
            var urls = gifResult.Data.Select(s => s.Url).Except(GlobalConfig.Config.UrlsToIgnore).ToList();
            await GlobalConfig.UrlCacheDal.Insert(urls);
        }
    }
}
