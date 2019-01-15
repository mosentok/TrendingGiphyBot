using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TrendingGiphyBotFunctions
{
    public static class RefreshImagesFunction
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        [FunctionName(nameof(RefreshImagesFunction))]
        public static async Task Run([TimerTrigger("%RefreshImagesFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var apiKey = Environment.GetEnvironmentVariable("GiphyApiKey");
            var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
            var commandTimeoutString = Environment.GetEnvironmentVariable("CommandTimeout");
            var commandTimeout = int.Parse(commandTimeoutString);
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            var requestUri = $"{trendingEndpoint}?api_key={apiKey}";
            var response = await _HttpClient.GetAsync(requestUri);
            var content = await response.Content.ReadAsStringAsync();
            var giphyResponse = JsonConvert.DeserializeObject<GiphyResponse>(content);
            int count;
            using (var context = new TrendingGiphyBotContext(connectionString, commandTimeout))
                count = await context.InsertNewTrendingGifs(giphyResponse.Data);
            log.LogInformation($"Inserted {count} URL caches.");
        }
    }
}
