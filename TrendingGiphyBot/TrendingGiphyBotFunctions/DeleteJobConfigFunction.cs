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
    public static class DeleteJobConfigFunction
    {
        [FunctionName(nameof(DeleteJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} deleting job config.");
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            using (var context = new TrendingGiphyBotContext(connectionString))
                await context.DeleteJobConfig(channelId);
            log.LogInformation($"Channel {channelId} deleted job config.");
            return new NoContentResult();
        }
    }
}
