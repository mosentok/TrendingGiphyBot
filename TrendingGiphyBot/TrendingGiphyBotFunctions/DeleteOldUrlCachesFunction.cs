using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class DeleteOldUrlCachesFunction
    {
        [FunctionName(nameof(DeleteOldUrlCachesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlCachesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var urlCachesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld");
            var urlCachesMaxDaysOld = int.Parse(urlCachesMaxDaysOldString);
            var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var deleteOldUrlCachesFunction = new DeleteOldUrlCachesFunction(log, context);
                await deleteOldUrlCachesFunction.RunAsync(oldestDate);
            }
        }
        readonly ILogger _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlCachesFunction(ILogger log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task RunAsync(DateTime oldestDate)
        {
            _Log.LogInformation($"Deleting URL caches older than {oldestDate}.");
            var count = await _Context.DeleteUrlCachesOlderThan(oldestDate);
            _Log.LogInformation($"Deleted {count} URL caches older than {oldestDate}.");
        }
    }
}
