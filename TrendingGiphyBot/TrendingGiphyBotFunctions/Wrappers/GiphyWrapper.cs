using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public class GiphyWrapper : IGiphyWrapper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        readonly string _GiphyTrendingEndpoint;
        readonly string _GiphyRandomEndpoint;
        public GiphyWrapper(string giphyTrendingEndpoint, string giphyRandomEndpoint)
        {
            _GiphyTrendingEndpoint = giphyTrendingEndpoint;
            _GiphyRandomEndpoint = giphyRandomEndpoint;
        }
        public async Task<GiphyTrendingResponse> GetTrendingGifsAsync()
        {
            string content;
            using (var response = await _HttpClient.GetAsync(_GiphyTrendingEndpoint))
                content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GiphyTrendingResponse>(content);
        }
        public async Task<GiphyRandomResponse> GetRandomGifAsync(string tag)
        {
            var combined = $"{_GiphyRandomEndpoint}&tag={tag}";
            var response = await _HttpClient.GetStringAsync(combined);
            try
            {
                return JsonConvert.DeserializeObject<GiphyRandomResponse>(response);
            }
            catch (JsonSerializationException ex)
            {
                throw new GiphyException($"Error getting {combined}", ex, response);
            }
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
