using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Containers;

namespace TrendingGiphyBot.Jobs
{
    class RefreshImagesJob : Job
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        internal RefreshImagesJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            var gifResult = await GetTrendingResult(GlobalConfig.GiphyTrendingEndpoint);
            var urls = gifResult.Data.Select(s => s.Url).Except(GlobalConfig.Config.UrlsToIgnore).ToList();
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                await entities.InsertUrlCaches(urls);
        }
        static async Task<GiphyTrendingResponse> GetTrendingResult(string requestUri)
        {
            var response = await _HttpClient.GetAsync(requestUri);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GiphyTrendingResponse>(content);
        }
    }
}
