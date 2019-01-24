using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions
{
    //TODO rename this to Gifs
    public static class RefreshImagesFunction
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        [FunctionName(nameof(RefreshImagesFunction))]
        public static async Task Run([TimerTrigger("%RefreshImagesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
            var response = await _HttpClient.GetAsync(trendingEndpoint);
            var content = await response.Content.ReadAsStringAsync();
            var giphyResponse = JsonConvert.DeserializeObject<GiphyTrendingResponse>(content);
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString))
                count = await context.InsertNewTrendingGifs(giphyResponse.Data);
            log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
