using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrendingGiphyBotCore.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                request.Headers.Add(headerName, headerValue);
                return await httpClient.SendAsync(request);
            }
        }
        public static async Task<HttpResponseMessage> PostWithHeaderAsync<T>(this HttpClient httpClient, string requestUri, T contentToSend, string headerName, string headerValue) where T : class
        {
            var serialized = JsonConvert.SerializeObject(contentToSend);
            using (var content = new StringContent(serialized))
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
            {
                request.Headers.Add(headerName, headerValue);
                return await httpClient.SendAsync(request);
            }
        }
        public static async Task<HttpResponseMessage> DeleteWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
            {
                request.Headers.Add(headerName, headerValue);
                return await httpClient.SendAsync(request);
            }
        }
    }
}
