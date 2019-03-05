using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class GetJobConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public GetJobConfigHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<JobConfigContainer> RunAsync(decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} getting job config.");
            var container = await _Context.GetJobConfig(channelId);
            _Log.LogInformation($"Channel {channelId} got job config.");
            return container;
        }
    }
}
