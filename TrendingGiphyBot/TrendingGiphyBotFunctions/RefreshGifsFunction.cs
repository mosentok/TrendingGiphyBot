using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            using (var giphyWrapper = new GiphyWrapper())
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var refreshGifsFunction = new RefreshGifsFunction(logWrapper, giphyWrapper, context);
                await refreshGifsFunction.RunAsync(trendingEndpoint);
            }
        }
        readonly ILoggerWrapper _Log;
        readonly IGiphyWrapper _GiphyWrapper;
        readonly ITrendingGiphyBotContext _Context;
        public RefreshGifsFunction(ILoggerWrapper log, IGiphyWrapper giphyWrapper, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _GiphyWrapper = giphyWrapper;
            _Context = context;
        }
        public async Task RunAsync(string trendingEndpoint)
        {
            var trendingResponse = await _GiphyWrapper.GetTrendingGifsAsync(trendingEndpoint);
            var count = await _Context.InsertNewTrendingGifs(trendingResponse.Data);
            _Log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
