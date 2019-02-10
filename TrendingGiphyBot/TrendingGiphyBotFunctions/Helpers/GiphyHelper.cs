using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class GiphyHelper : IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        public async Task<GiphyRandomResponse> GetRandomGif(string giphyRandomEndpoint, string tag)
        {
            var combined = $"{giphyRandomEndpoint}&tag={tag}";
            var response = await _HttpClient.GetStringAsync(combined);
            return JsonConvert.DeserializeObject<GiphyRandomResponse>(response);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
