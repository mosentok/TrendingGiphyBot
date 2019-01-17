using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Helpers
{
    public class FunctionHelper : IFunctionHelper, IDisposable
    {
        readonly HttpClient _HttpClient;
        readonly string _JobConfigEndpoint;
        public FunctionHelper(string functionKeyHeaderName, string jobConfigFunctionKey, string jobConfigEndpoint)
        {
            _HttpClient = new HttpClient();
            _HttpClient.DefaultRequestHeaders.Add(functionKeyHeaderName, jobConfigFunctionKey);
            _JobConfigEndpoint = jobConfigEndpoint;
        }
        public async Task<JobConfigContainer> GetJobConfigAsync(decimal channelId)
        {
            var response = await _HttpClient.GetAsync($"{_JobConfigEndpoint}/{channelId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JobConfigContainer>(content);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw new FunctionHelperException($"Error getting job config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
