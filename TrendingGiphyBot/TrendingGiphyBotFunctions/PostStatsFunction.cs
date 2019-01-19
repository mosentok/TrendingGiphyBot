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
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Posting stats.");
            var statPostsSerialized = Environment.GetEnvironmentVariable("StatPostsSerialized");
            var statPosts = JsonConvert.DeserializeObject<StatPost[]>(statPostsSerialized);
            var freshStat = await req.Body.ReadToEndAsync<FreshStat>();
            foreach (var statPost in statPosts)
                try
                {
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{freshStat.GuildCount}}}";
                    var requestUri = string.Format(statPost.UrlStringFormat, freshStat.BotId);
                    var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, content, "Authorization", statPost.Token);
                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        log.LogError($"Error posting stats. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'. Content '{message}'.");
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
