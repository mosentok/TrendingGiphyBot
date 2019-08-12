using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
            return await _HttpClient.GetWithHeaderAsync<JobConfigContainer>(requestUri, _FunctionsKeyHeaderName, _GetJobConfigFunctionKey);
        }
        public async Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            return await _HttpClient.PostWithHeaderAsync<JobConfigContainer>(requestUri, jobConfigContainer, _FunctionsKeyHeaderName, _PostJobConfigFunctionKey);
        }
        public async Task DeleteJobConfigAsync(decimal channelId)
        {
            var requestUri = $"{_JobConfigEndpoint}/{channelId}";
            await _HttpClient.DeleteWithHeaderAsync(requestUri, _FunctionsKeyHeaderName, _DeleteJobConfigFunctionKey);
        }
        public async Task PostStatsAsync(ulong botId, int guildCount)
        {
            var requestUri = $"{_PostStatsEndpoint}/{botId}";
            var container = new GuildCountContainer(guildCount);
            await _HttpClient.PostWithHeaderAsync(requestUri, container, _FunctionsKeyHeaderName, _PostStatsFunctionKey);
        }
        public async Task<Dictionary<decimal, string>> GetPrefixDictionaryAsync()
        {
            return await _HttpClient.GetWithHeaderAsync<Dictionary<decimal, string>>(_PrefixDictionaryEndpoint, _FunctionsKeyHeaderName, _GetPrefixDictionaryFunctionKey);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
