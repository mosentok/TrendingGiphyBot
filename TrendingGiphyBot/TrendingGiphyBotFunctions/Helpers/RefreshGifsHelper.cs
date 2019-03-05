using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class RefreshGifsHelper
    {
        readonly ILoggerWrapper _Log;
        readonly IGiphyWrapper _GiphyWrapper;
        readonly ITrendingGiphyBotContext _Context;
        public RefreshGifsHelper(ILoggerWrapper log, IGiphyWrapper giphyWrapper, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _GiphyWrapper = giphyWrapper;
            _Context = context;
        }
        public async Task RunAsync(string trendingEndpoint)
        {
            _Log.LogInformation("Getting trending gifs.");
            var trendingResponse = await _GiphyWrapper.GetTrendingGifsAsync(trendingEndpoint);
            _Log.LogInformation($"Got {trendingResponse.Data.Count} trending gifs.");
            _Log.LogInformation($"Inserting {trendingResponse.Data.Count} URL caches.");
            var count = await _Context.InsertNewTrendingGifs(trendingResponse.Data);
            _Log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
