using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public static class GetJobConfigFunction
    {
        [FunctionName(nameof(GetJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} getting job config.");
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            JobConfigContainer container;
            using (var context = new TrendingGiphyBotContext(connectionString))
                container = await context.GetJobConfig(channelId);
            log.LogInformation($"Channel {channelId} got job config.");
            return new OkObjectResult(container);
        }
    }
}
