using System.Net.Http;
using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostStringWithHeaderAsync(this HttpClient httpClient, string requestUri, string contentToSend, string headerName, string headerValue)
        {
            using (var content = new StringContent(contentToSend))
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content })
            {
                request.Headers.Add(headerName, headerValue);
                request.Headers.Add("Content-Type", "application/json");
                return await httpClient.SendAsync(request);
            }
        }
    }
}
