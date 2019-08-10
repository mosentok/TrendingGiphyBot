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
using TrendingGiphyBotFunctions.Exceptions;

namespace TrendingGiphyBotFunctions.Functions
{
    public class PostStatsFunction
    {
        readonly IStatWrapper _StatWrapper;
        public PostStatsFunction(IStatWrapper statWrapper)
        {
            _StatWrapper = statWrapper;
        }
        [FunctionName(nameof(PostStatsFunction))]
        //TODO change route to just "stats"
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "poststats/{botid:long}")] HttpRequest req, long botId, ILogger log)
        {
            var guildCountString = await req.Body.ReadToEndAsync();
            var guildCount = int.Parse(guildCountString);
            var statPostsSerialized = Environment.GetEnvironmentVariable("StatPostsSerialized");
            var statPosts = JsonConvert.DeserializeObject<List<StatPost>>(statPostsSerialized);
            log.LogInformation("Posting stats.");
            foreach (var statPost in statPosts)
                try
                {
                    var requestUri = string.Format(statPost.UrlStringFormat, botId);
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                    await _StatWrapper.PostStatAsync(requestUri, content, statPost.Token);
                }
                catch (StatPostException ex)
                {
                    log.LogError(ex, $"Error posting stats.");
                }
            log.LogInformation("Posted stats.");
            return new NoContentResult();
        }
    }
}
