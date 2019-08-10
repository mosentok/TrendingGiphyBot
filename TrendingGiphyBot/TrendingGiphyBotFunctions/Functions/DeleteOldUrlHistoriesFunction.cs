using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class DeleteOldUrlHistoriesFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlHistoriesFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(DeleteOldUrlHistoriesFunction))]
        public async Task Run([TimerTrigger("%DeleteOldUrlHistoriesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var oldestDate = _Context.GetUrlHistoriesOldestDate();
            log.LogInformation($"Deleting URL histories older than {oldestDate}.");
            var count = await _Context.DeleteUrlHistoriesOlderThan(oldestDate);
            log.LogInformation($"Deleted {count} URL histories older than {oldestDate}.");
        }
    }
}
