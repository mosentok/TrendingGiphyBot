using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class DeleteOldUrlHistoriesHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlHistoriesHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
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
