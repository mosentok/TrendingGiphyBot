using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotFunctions.Extensions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class PostJobConfigFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public PostJobConfigFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(PostJobConfigFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var container = await req.Body.ReadToEndAsync<JobConfigContainer>();
            log.LogInformation($"Channel {channelId} posting job config.");
            var result = await _Context.SetJobConfig(channelId, container);
            log.LogInformation($"Channel {channelId} posted job config.");
            return new OkObjectResult(result);
        }
    }
}
