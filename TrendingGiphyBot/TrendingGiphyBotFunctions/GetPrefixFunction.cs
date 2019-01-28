using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public static class GetPrefixFunction
    {
        [FunctionName(nameof(GetPrefixFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefix/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} getting prefix.");
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            string prefix;
            using (var context = new TrendingGiphyBotContext(connectionString))
                prefix = await context.GetPrefix(channelId);
            if (prefix != null)
            {
                log.LogInformation($"Channel {channelId} got prefix.");
                return new OkObjectResult(prefix);
            }
            log.LogInformation($"Channel {channelId} prefix not found.");
            return new NotFoundResult();
        }
    }
}
