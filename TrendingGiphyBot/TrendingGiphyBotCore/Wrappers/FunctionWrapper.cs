using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;
using TrendingGiphyBotCore.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Wrappers
{
    public class FunctionWrapper : IFunctionWrapper, IDisposable
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        readonly string _JobConfigEndpoint;
        readonly string _PostStatsEndpoint;
        readonly string _PrefixDictionaryEndpoint;
        readonly string _FunctionsKeyHeaderName;
        readonly string _GetJobConfigFunctionKey;
        readonly string _PostJobConfigFunctionKey;
        readonly string _DeleteJobConfigFunctionKey;
        readonly string _PostStatsFunctionKey;
        readonly string _GetPrefixDictionaryFunctionKey;
        public FunctionWrapper(IConfigurationWrapper config)
        {
            _JobConfigEndpoint = config["JobConfigEndpoint"];
            _PostStatsEndpoint = config["PostStatsEndpoint"];
            _PrefixDictionaryEndpoint = config["PrefixDictionaryEndpoint"];
            _FunctionsKeyHeaderName = config["FunctionsKeyHeaderName"];
            _GetJobConfigFunctionKey = config["GetJobConfigFunctionKey"];
            _PostJobConfigFunctionKey = config["PostJobConfigFunctionKey"];
            _DeleteJobConfigFunctionKey = config["DeleteJobConfigFunctionKey"];
            _PostStatsFunctionKey = config["PostStatsFunctionKey"];
            _GetPrefixDictionaryFunctionKey = config["GetPrefixDictionaryFunctionKey"];
        }
        public async Task<JobConfigContainer> GetJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            using (var response = await _HttpClient.GetWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _GetJobConfigFunctionKey))
                return await ProcessJobConfigResponse(channelId, response);
        }
        public async Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            using (var response = await _HttpClient.PostWithHeaderAsync(requestUri, jobConfigContainer, _FunctionsKeyHeaderName, _PostJobConfigFunctionKey))
                return await ProcessJobConfigResponse(channelId, response);
        }
        public async Task DeleteJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            var response = await _HttpClient.DeleteWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _DeleteJobConfigFunctionKey);
            response.Dispose();
        }
        static async Task<JobConfigContainer> ProcessJobConfigResponse(decimal channelId, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new FunctionHelperException($"Error with job config for channel '{channelId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JobConfigContainer>(content);
        }
        public async Task PostStatsAsync(ulong botId, int guildCount)
        {
            var requestUri = $"{_PostStatsEndpoint}/{botId}";
            var container = new GuildCountContainer(guildCount);
            using (var response = await _HttpClient.PostWithHeaderAsync(requestUri, container, _FunctionsKeyHeaderName, _PostStatsFunctionKey))
                if (!response.IsSuccessStatusCode)
                    throw new FunctionHelperException($"Error posting stats for bot '{botId}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'.");
        }
        public async Task<Dictionary<decimal, string>> GetPrefixDictionaryAsync()
        {
            string content;
            using (var response = await _HttpClient.GetWithHeaderAsync(_PrefixDictionaryEndpoint, _FunctionsKeyHeaderName, _GetPrefixDictionaryFunctionKey))
            {
                if (!response.IsSuccessStatusCode)
                    throw new FunctionHelperException($"Error getting prefix dictionary. Status code '{response.StatusCode}'. Reason phrase '{response.ReasonPhrase}'.");
                content = await response.Content.ReadAsStringAsync();
            }
            return JsonConvert.DeserializeObject<Dictionary<decimal, string>>(content);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
