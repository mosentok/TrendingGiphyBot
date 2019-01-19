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
        readonly HttpClient _GetJobConfigClient;
        readonly HttpClient _PostJobConfigClient;
        readonly HttpClient _GetPrefixClient;
        readonly HttpClient _PostPrefixClient;
        readonly string _JobConfigEndpoint;
        readonly string _PrefixEndpoint;
        public FunctionHelper(string jobConfigEndpoint, string prefixEndpoint, string functionsKeyHeaderName, string getJobConfigFunctionKey, string postJobConfigFunctionKey, string getPrefixFunctionKey, string postPrefixFunctionKey)
        {
            _GetJobConfigClient = new HttpClient();
            _GetJobConfigClient.DefaultRequestHeaders.Add(functionsKeyHeaderName, getJobConfigFunctionKey);
            _PostJobConfigClient = new HttpClient();
            _PostJobConfigClient.DefaultRequestHeaders.Add(functionsKeyHeaderName, postJobConfigFunctionKey);
            _GetPrefixClient = new HttpClient();
            _GetPrefixClient.DefaultRequestHeaders.Add(functionsKeyHeaderName, getPrefixFunctionKey);
            _PostPrefixClient = new HttpClient();
            _PostPrefixClient.DefaultRequestHeaders.Add(functionsKeyHeaderName, postPrefixFunctionKey);
            _JobConfigEndpoint = jobConfigEndpoint;
            _PrefixEndpoint = prefixEndpoint;
        }
        public async Task<JobConfigContainer> GetJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var response = await _GetJobConfigClient.GetAsync(requestUri);
            return await ProcessContainerResponse(channelId, response);
        }
        public async Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var serialized = JsonConvert.SerializeObject(jobConfigContainer);
            var content = new StringContent(serialized);
            var response = await _PostJobConfigClient.PostAsync(requestUri, content);
            return await ProcessContainerResponse(channelId, response);
        }
        public async Task<string> GetPrefixAsync(decimal channelId)
        {
            var requestUri = $"{_PrefixEndpoint}/{channelId}";
            var response = await _GetPrefixClient.GetAsync(requestUri);
            return await ProcessStringResponse(channelId, response);
        }
        public async Task<string> PostPrefixAsync(decimal channelId, string prefix)
        {
            var requestUri = $"{_PrefixEndpoint}/{channelId}";
            var content = new StringContent(prefix);
            var response = await _PostPrefixClient.PostAsync(requestUri, content);
            return await ProcessStringResponse(channelId, response);
        }
        static async Task<string> ProcessStringResponse(decimal channelId, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw new FunctionHelperException($"Error with prefix for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        static async Task<JobConfigContainer> ProcessContainerResponse(decimal channelId, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JobConfigContainer>(content);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw new FunctionHelperException($"Error with job config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public void Dispose()
        {
            _GetJobConfigClient?.Dispose();
            _PostJobConfigClient?.Dispose();
            _GetPrefixClient?.Dispose();
            _PostPrefixClient?.Dispose();
        }
    }
}
