using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class RefreshGifsFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        readonly IGiphyWrapper _GiphyWrapper;
        public RefreshGifsFunction(ITrendingGiphyBotContext context, IGiphyWrapper giphyWrapper)
        {
            _Context = context;
            _GiphyWrapper = giphyWrapper;
        }
        [FunctionName(nameof(RefreshGifsFunction))]
        public async Task Run([TimerTrigger("%RefreshGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var trendingResponse = await _GiphyWrapper.GetTrendingGifsAsync();
            var count = await _Context.InsertNewTrendingGifs(trendingResponse.Data);
            log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
