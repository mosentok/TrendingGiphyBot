using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotModel;
using System.Collections.Generic;

namespace TrendingGiphyBotFunctions
{
    public static class GetPrefixDictionaryFunction
    {
        [FunctionName(nameof(GetPrefixDictionaryFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "prefixDictionary")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting prefix dictionary.");
            Dictionary<decimal, string> prefixDictionary;
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            using (var context = new TrendingGiphyBotContext(connectionString))
                prefixDictionary = await context.GetPrefixDictionary();
            log.LogInformation($"Got {prefixDictionary.Count} prefixes.");
            return new OkObjectResult(prefixDictionary);
        }
    }
}
