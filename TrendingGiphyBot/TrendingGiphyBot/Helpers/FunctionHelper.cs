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
        static readonly HttpClient _JobConfigClient = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["jobConfigEndpoint"]) };
        static readonly HttpClient _PrefixClient = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["prefixEndpoint"]) };
        static readonly string _FunctionsKeyHeaderName = ConfigurationManager.AppSettings["functionsKeyHeaderName"];
        static readonly string _GetJobConfigFunctionKey = ConfigurationManager.AppSettings["getJobConfigFunctionKey"];
        static readonly string _PostJobConfigFunctionKey = ConfigurationManager.AppSettings["postJobConfigFunctionKey"];
        static readonly string _DeleteJobConfigFunctionKey = ConfigurationManager.AppSettings["deleteJobConfigFunctionKey"];
        static readonly string _GetPrefixFunctionKey = ConfigurationManager.AppSettings["getPrefixFunctionKey"];
        static readonly string _PostPrefixFunctionKey = ConfigurationManager.AppSettings["postPrefixFunctionKey"];
        public async Task<JobConfigContainer> GetJobConfigAsync(decimal channelId)
        {
            var response = await _JobConfigClient.GetWithHeaderAsync($"/{channelId}", _FunctionsKeyHeaderName, _GetJobConfigFunctionKey);
            return await ProcessContainerResponse(channelId, response);
        }
        public async Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var response = await _JobConfigClient.PostWithHeaderAsync($"/{channelId}", jobConfigContainer, _FunctionsKeyHeaderName, _PostJobConfigFunctionKey);
            return await ProcessContainerResponse(channelId, response);
        }
        public async Task DeleteJobConfigAsync(decimal channelId)
        {
            await _JobConfigClient.DeleteWithHeaderAsync($"/{channelId}", _FunctionsKeyHeaderName, _DeleteJobConfigFunctionKey);
        }
        public async Task<string> GetPrefixAsync(decimal channelId)
        {
            var response = await _PrefixClient.GetWithHeaderAsync($"/{channelId}", _FunctionsKeyHeaderName, _GetPrefixFunctionKey);
            return await ProcessStringResponse(channelId, response);
        }
        public async Task<string> PostPrefixAsync(decimal channelId, string prefix)
        {
            var response = await _PrefixClient.PostWithHeaderAsync($"/{channelId}", prefix, _FunctionsKeyHeaderName, _PostPrefixFunctionKey);
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
            _JobConfigClient?.Dispose();
            _PrefixClient?.Dispose();
        }
    }
}
