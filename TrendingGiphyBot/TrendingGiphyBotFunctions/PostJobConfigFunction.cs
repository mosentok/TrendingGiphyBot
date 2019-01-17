using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using TrendingGiphyBotFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace TrendingGiphyBotFunctions
{
    public static class PostJobConfigFunction
    {
        [FunctionName(nameof(PostJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            string content;
            using (var reader = new StreamReader(req.Body))
                content = await reader.ReadToEndAsync();
            var container = JsonConvert.DeserializeObject<JobConfigContainer>(content);
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            JobConfigContainer result;
            using (var context = new TrendingGiphyBotContext(connectionString))
                result = await context.SetJobConfig(channelId, container);
            return new OkObjectResult(result);
        }
    }
}
