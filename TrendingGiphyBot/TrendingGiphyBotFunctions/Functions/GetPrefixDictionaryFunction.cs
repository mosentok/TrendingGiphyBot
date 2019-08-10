using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public class GetPrefixDictionaryFunction
    {
        readonly ITrendingGiphyBotContext _Context;
        public GetPrefixDictionaryFunction(ITrendingGiphyBotContext context)
        {
            _Context = context;
        }
        [FunctionName(nameof(GetPrefixDictionaryFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefixDictionary")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting prefix dictionary.");
            var prefixDictionary = await _Context.GetPrefixDictionary();
            log.LogInformation($"Got {prefixDictionary.Count} prefixes.");
            return new OkObjectResult(prefixDictionary);
        }
    }
}
