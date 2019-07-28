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
        public async Task<GiphyTrendingResponse> GetTrendingGifsAsync(string giphyTrendingEndpoint)
        {
            var response = await _HttpClient.GetAsync(giphyTrendingEndpoint);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GiphyTrendingResponse>(content);
        }
        public async Task<GiphyRandomResponse> GetRandomGifAsync(string giphyRandomEndpoint, string tag)
        {
            var combined = $"{giphyRandomEndpoint}&tag={tag}";
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
