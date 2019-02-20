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
    public class GetJobConfigFunction
    {
        [FunctionName(nameof(GetJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var getJobConfigFunction = new GetJobConfigFunction(log, context);
                return await getJobConfigFunction.RunAsync(channelId);
            }
        }
        readonly ILogger _Log;
        readonly ITrendingGiphyBotContext _Context;
        public GetJobConfigFunction(ILogger log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<IActionResult> RunAsync(decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} getting job config.");
            var container = await _Context.GetJobConfig(channelId);
            _Log.LogInformation($"Channel {channelId} got job config.");
            return new OkObjectResult(container);
        }
    }
}
