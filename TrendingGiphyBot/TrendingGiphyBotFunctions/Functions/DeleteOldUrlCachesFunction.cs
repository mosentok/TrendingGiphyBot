using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class DeleteOldUrlCachesFunction
    {
        [FunctionName(nameof(DeleteOldUrlCachesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlCachesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var urlCachesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld");
            var urlCachesMaxDaysOld = int.Parse(urlCachesMaxDaysOldString);
            var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var deleteOldUrlCachesHelper = new DeleteOldUrlCachesHelper(logWrapper, context);
                await deleteOldUrlCachesHelper.RunAsync(oldestDate);
            }
        }
    }
}
