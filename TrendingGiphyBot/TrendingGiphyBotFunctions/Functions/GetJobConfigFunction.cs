using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotFunctions.Helpers;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class GetJobConfigFunction
    {
        [FunctionName(nameof(GetJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var getJobConfigHelper = new GetJobConfigHelper(logWrapper, context);
                return await getJobConfigHelper.RunAsync(channelId);
            }
        }
    }
}
