using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class PostJobConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public PostJobConfigHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<IActionResult> RunAsync(JobConfigContainer container, decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} posting job config.");
            var result = await _Context.SetJobConfig(channelId, container);
            _Log.LogInformation($"Channel {channelId} posted job config.");
            return new OkObjectResult(result);
        }
    }
}
