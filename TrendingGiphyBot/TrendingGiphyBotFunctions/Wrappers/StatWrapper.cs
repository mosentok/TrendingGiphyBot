using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public class StatWrapper : IStatWrapper, IDisposable
    {
        readonly HttpClient _HttpClient = new HttpClient();
        readonly List<StatPost> _StatPosts;
        public StatWrapper(List<StatPost> statPosts)
        {
            _StatPosts = statPosts;
        }
        public async Task PostStatsAsync(long botId, int guildCount, ILogger log)
        {
            foreach (var statPost in _StatPosts)
            {
                var requestUri = string.Format(statPost.UrlStringFormat, botId);
                var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                using (var stringContent = new StringContent(content, Encoding.UTF8, "application/json"))
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = stringContent })
                {
                    request.Headers.Add("Authorization", statPost.Token);
                    using (var response = await _HttpClient.SendAsync(request))
                        if (!response.IsSuccessStatusCode)
                            log.LogError($"Error: {request.Method.Method} {request.RequestUri.AbsoluteUri} {response.StatusCode.ToString()} {response.ReasonPhrase}");
                }
            }
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
