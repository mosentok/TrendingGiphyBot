using System;
using System.Net.Http;

namespace TrendingGiphyBot.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpClient WithBaseAddress(this HttpClient httpClient, string baseAddress)
        {
            httpClient.BaseAddress = new Uri(baseAddress);
            return httpClient;
        }
        public static HttpClient WithDefaultRequestHeader(this HttpClient httpClient, string name, string value)
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
            return httpClient;
        }
    }
}
