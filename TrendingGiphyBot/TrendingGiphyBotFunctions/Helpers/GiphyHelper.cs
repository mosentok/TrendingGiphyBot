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
        public Task<GiphyRandomResponse> GetRandomGif(string giphyRandomEndpoint, string tag) => GetRandomGif($"{giphyRandomEndpoint}&tag={tag}");
        public async Task<GiphyRandomResponse> GetRandomGif(string giphyRandomEndpoint)
        {
            var response = await _HttpClient.GetStringAsync(giphyRandomEndpoint);
            return JsonConvert.DeserializeObject<GiphyRandomResponse>(response);
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
