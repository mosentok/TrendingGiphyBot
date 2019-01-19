using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions
{
    public static class DeleteOldUrlHistoriesFunction
    {
        [FunctionName(nameof(DeleteOldUrlHistoriesFunction))]
        public static async Task Run([TimerTrigger("%DeleteOldUrlHistoriesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var urlHistoriesMaxDaysOld = int.Parse(Environment.GetEnvironmentVariable("UrlHistoriesMaxDaysOld"));
            var oldestDate = DateTime.Now.AddDays(-urlHistoriesMaxDaysOld);
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString))
                count = await context.DeleteUrlHistoriesOlderThan(oldestDate);
            log.LogInformation($"Deleted {count} URL histories older than {oldestDate}.");
        }
    }
}
