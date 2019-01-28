using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Extensions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public static class PostPrefixFunction
    {
        [FunctionName(nameof(PostPrefixFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "prefix/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} posting prefix.");
            var prefix = await req.Body.ReadToEndAsync();
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            string result;
            using (var context = new TrendingGiphyBotContext(connectionString))
                result = await context.SetPrefix(channelId, prefix);
            log.LogInformation($"Channel {channelId} posted prefix.");
            return new OkObjectResult(result);
        }
    }
}
