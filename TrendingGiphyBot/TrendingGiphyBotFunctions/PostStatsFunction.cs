using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Extensions;
using System.Collections.Generic;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotFunctions
{
    public class PostStatsFunction
    {
        [FunctionName(nameof(PostStatsFunction))]
        //TODO change route to just "stats"
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "poststats/{botid:long}")] HttpRequest req, long botId, ILogger log)
        {
            var guildCountString = await req.Body.ReadToEndAsync();
            var guildCount = int.Parse(guildCountString);
            var statPostsSerialized = Environment.GetEnvironmentVariable("StatPostsSerialized");
            var statPosts = JsonConvert.DeserializeObject<List<StatPost>>(statPostsSerialized);
            var logWrapper = new LoggerWrapper(log);
            using (var statWrapper = new StatWrapper())
            {
                var postStatsFunction = new PostStatsFunction(logWrapper, statWrapper);
                return await postStatsFunction.RunAsync(guildCount, botId, statPosts);
            }
        }
        readonly ILoggerWrapper _Log;
        readonly IStatWrapper _StatWrapper;
        public PostStatsFunction(ILoggerWrapper log, IStatWrapper statWrapper)
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
