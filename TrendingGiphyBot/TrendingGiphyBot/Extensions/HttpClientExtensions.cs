using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> GetWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add(headerName, headerValue);
            return httpClient.SendAsync(request);
        }
        public static Task<HttpResponseMessage> PostStringWithHeaderAsync(this HttpClient httpClient, string requestUri, string contentToSend, string headerName, string headerValue)
        {
            var content = new StringContent(contentToSend);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            request.Headers.Add(headerName, headerValue);
            return httpClient.SendAsync(request);
        }
        public static Task<HttpResponseMessage> PostObjectWithHeaderAsync<T>(this HttpClient httpClient, string requestUri, T contentToSend, string headerName, string headerValue) where T : class
        {
            var serialized = JsonConvert.SerializeObject(contentToSend);
            var content = new StringContent(serialized);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            request.Headers.Add(headerName, headerValue);
            return httpClient.SendAsync(request);
        }
        public static Task<HttpResponseMessage> DeleteWithHeaderAsync(this HttpClient httpClient, string requestUri, string headerName, string headerValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Headers.Add(headerName, headerValue);
            return httpClient.SendAsync(request);
        }
    }
}
