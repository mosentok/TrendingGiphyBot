using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class RefreshGifsFunction
    {
        [FunctionName(nameof(RefreshGifsFunction))]
        public static async Task Run([TimerTrigger("%RefreshGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
            var logWrapper = new LoggerWrapper(log);
            using (var giphyHelper = new GiphyHelper())
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var refreshGifsFunction = new RefreshGifsFunction(logWrapper, giphyHelper, context);
                await refreshGifsFunction.RunAsync(trendingEndpoint);
            }
        }
        readonly ILoggerWrapper _Log;
        readonly IGiphyHelper _GiphyHelper;
        readonly ITrendingGiphyBotContext _Context;
        public RefreshGifsFunction(ILoggerWrapper log, IGiphyHelper giphyHelper, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _GiphyHelper = giphyHelper;
            _Context = context;
        }
        public async Task RunAsync(string trendingEndpoint)
        {
            var trendingResponse = await _GiphyHelper.GetTrendingGifsAsync(trendingEndpoint);
            var count = await _Context.InsertNewTrendingGifs(trendingResponse.Data);
            _Log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
