using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotCore.Exceptions;

namespace TrendingGiphyBotCore.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                return await httpClient.SendWithHeaderAsync(headerName, headerValue, request);
        }
        public static async Task<HttpResponseMessage> PostWithHeaderAsync<T>(this HttpClient httpClient, string requestUri, T contentToSend, string headerName, string headerValue) where T : class
        {
            var serialized = JsonConvert.SerializeObject(contentToSend);
            using (var content = new StringContent(serialized))
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
                return await httpClient.SendWithHeaderAsync(headerName, headerValue, request);
        }
        public static async Task<HttpResponseMessage> DeleteWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
                return await httpClient.SendWithHeaderAsync(headerName, headerValue, request);
        }
        static async Task<HttpResponseMessage> SendWithHeaderAsync(this HttpClient httpClient, string headerName, string headerValue, HttpRequestMessage request)
        {
            request.Headers.Add(headerName, headerValue);
            using (var response = await httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                    throw new FunctionException(request, response);
                return response;
            }
        }
    }
}
