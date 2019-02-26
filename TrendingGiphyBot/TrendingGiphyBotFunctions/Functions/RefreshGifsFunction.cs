using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class RefreshGifsFunction
    {
        [FunctionName(nameof(RefreshGifsFunction))]
        public static async Task Run([TimerTrigger("%RefreshGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
            var logWrapper = new LoggerWrapper(log);
            using (var giphyWrapper = new GiphyWrapper())
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var refreshGifsHelper = new RefreshGifsHelper(logWrapper, giphyWrapper, context);
                await refreshGifsHelper.RunAsync(trendingEndpoint);
            }
        }
    }
}
