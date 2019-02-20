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
using TrendingGiphyBotFunctions.Helpers;
using System.Collections.Generic;
using TrendingGiphyBotFunctions.Exceptions;

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
            using (var statHelper = new StatHelper())
            {
                var postStatsFunction = new PostStatsFunction(log, statHelper);
                return await postStatsFunction.RunAsync(guildCount, botId, statPosts);
            }
        }
        readonly ILogger _Log;
        readonly IStatHelper _StatHelper;
        public PostStatsFunction(ILogger log, IStatHelper statHelper)
        {
            _Log = log;
            _StatHelper = statHelper;
        }
        public async Task<IActionResult> RunAsync(int guildCount, long botId, List<StatPost> statPosts)
        {
            _Log.LogInformation("Posting stats.");
            foreach (var statPost in statPosts)
                try
                {
                    var requestUri = string.Format(statPost.UrlStringFormat, botId);
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                    await _StatHelper.PostStatAsync(requestUri, content, statPost.Token);
                }
                catch (StatPostException ex)
                {
                    _Log.LogError(ex.ToString());
                }
            _Log.LogInformation("Posted stats.");
            return new NoContentResult();
        }
    }
}
