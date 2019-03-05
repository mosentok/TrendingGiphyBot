using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotFunctions.Extensions;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotFunctions.Helpers;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class PostJobConfigFunction
    {
        [FunctionName(nameof(PostJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var container = await req.Body.ReadToEndAsync<JobConfigContainer>();
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            JobConfigContainer result;
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var postJobConfigHelper = new PostJobConfigHelper(logWrapper, context);
                result = await postJobConfigHelper.RunAsync(container, channelId);
            }
            return new OkObjectResult(result);
        }
    }
}
