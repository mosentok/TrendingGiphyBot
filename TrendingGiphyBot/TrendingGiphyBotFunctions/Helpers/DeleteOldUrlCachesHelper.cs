using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class DeleteOldUrlCachesHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteOldUrlCachesHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
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
