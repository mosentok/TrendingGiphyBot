using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Helpers
{
    public class FunctionHelper : IFunctionHelper, IDisposable
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        readonly string _JobConfigEndpoint;
        readonly string _PrefixEndpoint;
        readonly string _PostStatsEndpoint;
        readonly string _FunctionsKeyHeaderName;
        readonly string _GetJobConfigFunctionKey;
        readonly string _PostJobConfigFunctionKey;
        readonly string _DeleteJobConfigFunctionKey;
        readonly string _PostStatsFunctionKey;
        readonly string _GetPrefixFunctionKey;
        readonly string _PostPrefixFunctionKey;
        public FunctionHelper(IConfiguration config)
        {
            _JobConfigEndpoint = config["jobConfigEndpoint"];
            _PrefixEndpoint = config["prefixEndpoint"];
            _PostStatsEndpoint = config["postStatsEndpoint"];
            _FunctionsKeyHeaderName = config["functionsKeyHeaderName"];
            _GetJobConfigFunctionKey = config["getJobConfigFunctionKey"];
            _PostJobConfigFunctionKey = config["postJobConfigFunctionKey"];
            _DeleteJobConfigFunctionKey = config["deleteJobConfigFunctionKey"];
            _PostStatsFunctionKey = config["postStatsFunctionKey"];
            _GetPrefixFunctionKey = config["getPrefixFunctionKey"];
            _PostPrefixFunctionKey = config["postPrefixFunctionKey"];
        }
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
            throw new FunctionHelperException($"Error with job config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public async Task PostStatsAsync(ulong botId, int guildCount)
        {
            var requestUri = $"{_PostStatsEndpoint}/{botId}";
            var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, guildCount.ToString(), _FunctionsKeyHeaderName, _PostStatsFunctionKey);
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error posting stats for bot '{botId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
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
            throw new FunctionHelperException($"Error with prefix for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
