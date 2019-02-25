using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class DeleteOldUrlHistoriesFunction
    {
        [FunctionName(nameof(DeleteOldUrlHistoriesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlHistoriesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var urlHistoriesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlHistoriesMaxDaysOld");
            var urlHistoriesMaxDaysOld = int.Parse(urlHistoriesMaxDaysOldString);
            var oldestDate = DateTime.Now.AddDays(-urlHistoriesMaxDaysOld);
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var deleteOldUrlHistoriesFunction = new DeleteOldUrlHistoriesFunction(logWrapper, context);
                await deleteOldUrlHistoriesFunction.RunAsync(oldestDate);
            }
        }
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlHistoriesFunction(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task RunAsync(DateTime oldestDate)
        {
            _Log.LogInformation($"Deleting URL histories older than {oldestDate}.");
            var count = await _Context.DeleteUrlHistoriesOlderThan(oldestDate);
            _Log.LogInformation($"Deleted {count} URL histories older than {oldestDate}.");
        }
    }
}
