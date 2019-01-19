using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using TrendingGiphyBotFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotFunctions.Extensions;

namespace TrendingGiphyBotFunctions
{
    public static class PostJobConfigFunction
    {
        [FunctionName(nameof(PostJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} posting job config.");
            var container = await req.Body.ReadToEndAsync<JobConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            JobConfigContainer result;
            using (var context = new TrendingGiphyBotContext(connectionString))
                result = await context.SetJobConfig(channelId, container);
            log.LogInformation($"Channel {channelId} posted job config.");
            return new OkObjectResult(result);
        }
    }
}
