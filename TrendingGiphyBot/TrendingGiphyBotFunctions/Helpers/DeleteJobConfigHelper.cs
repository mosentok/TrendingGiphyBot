using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class DeleteJobConfigHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteJobConfigHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task RunAsync(decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} deleting job config.");
            await _Context.DeleteJobConfig(channelId);
            _Log.LogInformation($"Channel {channelId} deleted job config.");
        }
    }
}
