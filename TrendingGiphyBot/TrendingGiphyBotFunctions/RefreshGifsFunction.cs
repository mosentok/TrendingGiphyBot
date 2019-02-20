using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public static class RefreshGifsFunction
    {
        [FunctionName(nameof(RefreshGifsFunction))]
        public static async Task Run([TimerTrigger("%RefreshGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
            GiphyTrendingResponse giphyResponse;
            using (var giphyHelper = new GiphyHelper())
                giphyResponse = await giphyHelper.GetTrendingGifsAsync(trendingEndpoint);
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString))
                count = await context.InsertNewTrendingGifs(giphyResponse.Data);
            log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
