using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using TrendingGiphyBotFunctions.Models;
using Microsoft.AspNetCore.Mvc;

namespace TrendingGiphyBotFunctions
{
    public static class GetJobConfigFunction
    {
        [FunctionName(nameof(GetJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            JobConfigContainer container;
            using (var context = new TrendingGiphyBotContext(connectionString))
                container = await context.GetJobConfig(channelId);
            if (container != null)
                return new OkObjectResult(container);
            return new NotFoundResult();
        }
    }
}
