using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotFunctions
{
    public class DeleteJobConfigFunction
    {
        [FunctionName(nameof(DeleteJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var deleteJobConfigFunction = new DeleteJobConfigFunction(logWrapper, context);
                return await deleteJobConfigFunction.RunAsync(channelId);
            }
        }
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public DeleteJobConfigFunction(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<IActionResult> RunAsync(decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} deleting job config.");
            await _Context.DeleteJobConfig(channelId);
            _Log.LogInformation($"Channel {channelId} deleted job config.");
            return new NoContentResult();
        }
    }
}
