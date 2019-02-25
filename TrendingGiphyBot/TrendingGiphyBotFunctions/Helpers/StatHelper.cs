﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Extensions;

namespace TrendingGiphyBotFunctions.Helpers
{
    //TODO rename this to Wrapper
    public class StatHelper : IStatHelper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        public async Task PostStatAsync(string requestUri, string content, string token)
        {
            var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, content, "Authorization", token);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new StatPostException($"Error posting stats. Request uri '{requestUri}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'. Content '{message}'.");
            }
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
