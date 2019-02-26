using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class DeleteOldUrlHistoriesFunction
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
                var deleteOldUrlHistoriesHelper = new DeleteOldUrlHistoriesHelper(logWrapper, context);
                await deleteOldUrlHistoriesHelper.RunAsync(oldestDate);
            }
        }
    }
}
