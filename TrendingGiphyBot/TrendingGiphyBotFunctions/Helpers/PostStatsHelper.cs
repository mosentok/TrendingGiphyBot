using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class PostStatsHelper
    {
        readonly ILoggerWrapper _Log;
        readonly IStatWrapper _StatWrapper;
        public PostStatsHelper(ILoggerWrapper log, IStatWrapper statWrapper)
        {
            _Log = log;
            _StatWrapper = statWrapper;
        }
        public async Task<IActionResult> RunAsync(int guildCount, long botId, List<StatPost> statPosts)
        {
            _Log.LogInformation("Posting stats.");
            foreach (var statPost in statPosts)
                try
                {
                    var requestUri = string.Format(statPost.UrlStringFormat, botId);
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                    await _StatWrapper.PostStatAsync(requestUri, content, statPost.Token);
                }
                catch (StatPostException ex)
                {
                    _Log.LogError(ex, $"Error posting stats.");
                }
            _Log.LogInformation("Posted stats.");
            return new NoContentResult();
        }
    }
}
