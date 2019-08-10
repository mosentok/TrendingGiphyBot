using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class DeleteOldUrlCachesFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlCachesFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(DeleteOldUrlCachesFunction))]
        public async Task Run([TimerTrigger("%DeleteOldUrlCachesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var urlCachesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld");
            var urlCachesMaxDaysOld = int.Parse(urlCachesMaxDaysOldString);
            var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            log.LogInformation($"Deleting URL caches older than {oldestDate}.");
            var count = await _Context.DeleteUrlCachesOlderThan(oldestDate);
            log.LogInformation($"Deleted {count} URL caches older than {oldestDate}.");
        }
    }
}
