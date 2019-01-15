using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TrendingGiphyBotFunctions
{
    public static class DeleteOldUrlCachesFunction
    {
        [FunctionName(nameof(DeleteOldUrlCachesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlCachesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var urlCachesMaxDaysOld = int.Parse(Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld"));
            var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            var commandTimeoutString = Environment.GetEnvironmentVariable("CommandTimeout");
            var commandTimeout = int.Parse(commandTimeoutString);
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString, commandTimeout))
                count = await context.DeleteUrlCachesOlderThan(oldestDate);
            log.LogInformation($"Deleted {count} URL caches older than {oldestDate}.");
        }
    }
}
