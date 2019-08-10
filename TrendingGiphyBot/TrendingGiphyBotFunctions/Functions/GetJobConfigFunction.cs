using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class GetJobConfigFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public GetJobConfigFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(GetJobConfigFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} getting job config.");
            var container = await _Context.GetJobConfig(channelId);
            log.LogInformation($"Channel {channelId} got job config.");
            return new OkObjectResult(container);
        }
    }
}
