using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public static class DeleteOldUrlCachesFunction
    {
        [FunctionName(nameof(DeleteOldUrlCachesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlCachesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var urlCachesMaxDaysOld = int.Parse(Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld"));
            var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString))
                count = await context.DeleteUrlCachesOlderThan(oldestDate);
            log.LogInformation($"Deleted {count} URL caches older than {oldestDate}.");
        }
    }
}
