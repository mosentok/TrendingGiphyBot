using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class DeleteJobConfigFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public DeleteJobConfigFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(DeleteJobConfigFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            log.LogInformation($"Channel {channelId} deleting job config.");
            await _Context.DeleteJobConfig(channelId);
            log.LogInformation($"Channel {channelId} deleted job config.");
            return new NoContentResult();
        }
    }
}
