using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Extensions;

namespace TrendingGiphyBotFunctions
{
    public static class PostStatsFunction
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        [FunctionName(nameof(PostStatsFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "poststats/{botid:long}")] HttpRequest req, long botId, ILogger log)
        {
            log.LogInformation("Posting stats.");
            var statPostsSerialized = Environment.GetEnvironmentVariable("StatPostsSerialized");
            var statPosts = JsonConvert.DeserializeObject<StatPost[]>(statPostsSerialized);
            var guildCountString = await req.Body.ReadToEndAsync();
            var guildCount = int.Parse(guildCountString);
            foreach (var statPost in statPosts)
                try
                {
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                    var requestUri = string.Format(statPost.UrlStringFormat, botId);
                    var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, content, "Authorization", statPost.Token);
                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        log.LogError($"Error posting stats. Request uri '{requestUri}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'. Content '{message}'.");
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                }
            log.LogInformation("Posted stats.");
            return new NoContentResult();
        }
    }
}
