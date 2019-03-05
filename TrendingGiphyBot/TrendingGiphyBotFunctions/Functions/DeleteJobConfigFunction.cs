using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotFunctions.Helpers;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class DeleteJobConfigFunction
    {
        [FunctionName(nameof(DeleteJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var deleteJobConfigHelper = new DeleteJobConfigHelper(logWrapper, context);
                await deleteJobConfigHelper.RunAsync(channelId);
            }
            return new NoContentResult();
        }
    }
}
