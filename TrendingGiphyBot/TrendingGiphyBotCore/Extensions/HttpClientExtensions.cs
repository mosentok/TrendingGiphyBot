using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotCore.Exceptions;

namespace TrendingGiphyBotCore.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetWithHeaderAsync<T>(this HttpClient httpClient, string requestUri, string headerName, string headerValue) where T : class
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                return await httpClient.SendWithHeaderAsync<T>(headerName, headerValue, request);
        }
        public static async Task PostWithHeaderAsync(this HttpClient httpClient, string requestUri, object contentToSend, string headerName, string headerValue)
        {
            var serialized = JsonConvert.SerializeObject(contentToSend);
            using (var content = new StringContent(serialized))
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
                await httpClient.SendWithHeaderAsync(headerName, headerValue, request);
        }
        public static async Task<T> PostWithHeaderAsync<T>(this HttpClient httpClient, string requestUri, object contentToSend, string headerName, string headerValue) where T : class
        {
            var serialized = JsonConvert.SerializeObject(contentToSend);
            using (var content = new StringContent(serialized))
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
                return await httpClient.SendWithHeaderAsync<T>(headerName, headerValue, request);
        }
        public static async Task DeleteWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
                await httpClient.SendWithHeaderAsync(headerName, headerValue, request);
        }
        static async Task SendWithHeaderAsync(this HttpClient httpClient, string headerName, string headerValue, HttpRequestMessage request)
        {
            request.Headers.Add(headerName, headerValue);
            using (var response = await httpClient.SendAsync(request))
                if (!response.IsSuccessStatusCode)
                    throw new FunctionException(request, response);
        }
        static async Task<T> SendWithHeaderAsync<T>(this HttpClient httpClient, string headerName, string headerValue, HttpRequestMessage request) where T : class
        {
            request.Headers.Add(headerName, headerValue);
            using (var response = await httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                    throw new FunctionException(request, response);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
