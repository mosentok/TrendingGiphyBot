using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotFunctions.Helpers;
using System.Collections.Generic;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class GetPrefixDictionaryFunction
    {
        [FunctionName(nameof(GetPrefixDictionaryFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefixDictionary")] HttpRequest req, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var logWrapper = new LoggerWrapper(log);
            Dictionary<decimal, string> prefixDictionary;
            using (var context = new TrendingGiphyBotContext(connectionString))
            {
                var getPrefixDictionaryHelper = new GetPrefixDictionaryHelper(logWrapper, context);
                prefixDictionary = await getPrefixDictionaryHelper.RunAsync();
            }
            return new OkObjectResult(prefixDictionary);
        }
    }
}
