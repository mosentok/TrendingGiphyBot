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
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotFunctions.Helpers;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class PostStatsFunction
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
                var postStatsHelper = new PostStatsHelper(logWrapper, statWrapper);
                await postStatsHelper.RunAsync(guildCount, botId, statPosts);
            }
            return new NoContentResult();
        }
    }
}
