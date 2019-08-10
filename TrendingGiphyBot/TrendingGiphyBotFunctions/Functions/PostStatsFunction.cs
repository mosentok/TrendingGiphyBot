using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Extensions;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotFunctions.Functions
{
    public class PostStatsFunction
    {
        readonly IStatWrapper _StatWrapper;
        public PostStatsFunction(IStatWrapper statWrapper)
        {
            _StatWrapper = statWrapper;
        }
        [FunctionName(nameof(PostStatsFunction))]
        //TODO change route to just "stats"
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "poststats/{botid:long}")] HttpRequest req, long botId, ILogger log)
        {
            var guildCountString = await req.Body.ReadToEndAsync();
            var guildCount = int.Parse(guildCountString);
            log.LogInformation("Posting stats.");
            await _StatWrapper.PostStatsAsync(botId, guildCount, log);
            log.LogInformation("Posted stats.");
            return new NoContentResult();
        }
    }
}
