using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotFunctions
{
    public class GetPrefixDictionaryFunction
    {
        [FunctionName(nameof(GetPrefixDictionaryFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefixDictionary")] HttpRequest req, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var getPrefixDictionaryFunction = new GetPrefixDictionaryFunction(logWrapper, context);
                return await getPrefixDictionaryFunction.RunAsync();
            }
        }
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public GetPrefixDictionaryFunction(ILoggerWrapper log, ITrendingGiphyBotContext context)
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
