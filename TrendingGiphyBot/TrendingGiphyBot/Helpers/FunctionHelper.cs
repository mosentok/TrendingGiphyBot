using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Exceptions;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Helpers
{
    public class FunctionHelper : IFunctionHelper, IDisposable
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        static readonly string _JobConfigEndpoint = ConfigurationManager.AppSettings["jobConfigEndpoint"];
        static readonly string _PrefixEndpoint = ConfigurationManager.AppSettings["prefixEndpoint"];
        static readonly string _FunctionsKeyHeaderName = ConfigurationManager.AppSettings["functionsKeyHeaderName"];
        static readonly string _GetJobConfigFunctionKey = ConfigurationManager.AppSettings["getJobConfigFunctionKey"];
        static readonly string _PostJobConfigFunctionKey = ConfigurationManager.AppSettings["postJobConfigFunctionKey"];
        static readonly string _DeleteJobConfigFunctionKey = ConfigurationManager.AppSettings["deleteJobConfigFunctionKey"];
        static readonly string _GetPrefixFunctionKey = ConfigurationManager.AppSettings["getPrefixFunctionKey"];
        static readonly string _PostPrefixFunctionKey = ConfigurationManager.AppSettings["postPrefixFunctionKey"];
        public async Task<JobConfigContainer> GetJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var response = await _HttpClient.GetWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _GetJobConfigFunctionKey);
            return await ProcessJobConfigResponse(channelId, response);
        }
        public async Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var response = await _HttpClient.PostObjectWithHeaderAsync(requestUri, jobConfigContainer, _FunctionsKeyHeaderName, _PostJobConfigFunctionKey);
            return await ProcessJobConfigResponse(channelId, response);
        }
        public async Task DeleteJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            await _HttpClient.DeleteWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _DeleteJobConfigFunctionKey);
        }
        static async Task<JobConfigContainer> ProcessJobConfigResponse(decimal channelId, HttpResponseMessage response)
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
        public async Task<string> GetPrefixAsync(decimal channelId)
        {
            var requestUri = $"{_PrefixEndpoint}/{channelId}";
            var response = await _HttpClient.GetWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _GetPrefixFunctionKey);
            return await ProcessPrefixResponse(channelId, response);
        }
        public async Task<string> PostPrefixAsync(decimal channelId, string prefix)
        {
            var requestUri = $"{_PrefixEndpoint}/{channelId}";
            var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, prefix, _FunctionsKeyHeaderName, _PostPrefixFunctionKey);
            return await ProcessPrefixResponse(channelId, response);
        }
        static async Task<string> ProcessPrefixResponse(decimal channelId, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw new FunctionHelperException($"Error with prefix for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
