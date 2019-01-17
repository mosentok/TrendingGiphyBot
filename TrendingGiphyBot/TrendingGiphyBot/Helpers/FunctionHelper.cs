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
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var response = await _HttpClient.GetAsync(requestUri);
            return await ProcessResponse(channelId, response);
        }
        public async Task<JobConfigContainer> SetJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var serialized = JsonConvert.SerializeObject(jobConfigContainer);
            var content = new StringContent(serialized);
            var response = await _HttpClient.PostAsync(requestUri, content);
            return await ProcessResponse(channelId, response);
        }
        static async Task<JobConfigContainer> ProcessResponse(decimal channelId, HttpResponseMessage response)
        {
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
