using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TrendingGiphyBot.Wordnik.Models;

namespace TrendingGiphyBot.Wordnik.Clients
{
    class WordnikClient : IDisposable
    {
        readonly HttpClient _HttpClient;
        readonly string _Token;
        public WordnikClient(string baseAddress, string token) : this(new Uri(baseAddress), token) { }
        public WordnikClient(Uri baseAddress, string token)
        {
            _HttpClient = new HttpClient { BaseAddress = baseAddress };
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _Token = token;
        }
        public async Task<WordOfTheDay> GetWordOfTheDay()
        {
            var wordOfTheDayUrl = "http://api.wordnik.com:80/v4/words.json/wordOfTheDay?api_key=" + _Token;
            var response = await _HttpClient.GetAsync(wordOfTheDayUrl);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WordOfTheDay>(stringResponse);
            }
            //TODO how to handle errors
            return null;
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
