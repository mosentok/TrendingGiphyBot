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
    public class GetPrefixDictionaryFunction
    {
        [FunctionName(nameof(GetPrefixDictionaryFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefixDictionary")] HttpRequest req, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var getPrefixDictionaryFunction = new GetPrefixDictionaryFunction(log, new TrendingGiphyBotContext(connectionString));
            return await getPrefixDictionaryFunction.RunAsync();
        }
        readonly ILogger _Log;
        readonly ITrendingGiphyBotContext _Context;
        public GetPrefixDictionaryFunction(ILogger log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<IActionResult> RunAsync()
        {
            _Log.LogInformation("Getting prefix dictionary.");
            var prefixDictionary = await _Context.GetPrefixDictionary();
            _Log.LogInformation($"Got {prefixDictionary.Count} prefixes.");
            return new OkObjectResult(prefixDictionary);
        }
    }
}
