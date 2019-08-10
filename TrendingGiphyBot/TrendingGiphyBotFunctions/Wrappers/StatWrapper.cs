using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Extensions;
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
                try
                {
                    var requestUri = string.Format(statPost.UrlStringFormat, botId);
                    var content = $"{{\"{statPost.GuildCountPropertyName}\":{guildCount}}}";
                    await PostStatAsync(requestUri, content, statPost.Token);
                }
                catch (StatPostException ex)
                {
                    log.LogError(ex, $"Error posting stats.");
                }
        }
        async Task PostStatAsync(string requestUri, string content, string token)
        {
            using (var response = await _HttpClient.PostStringWithHeaderAsync(requestUri, content, "Authorization", token))
                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new StatPostException($"Error posting stats. Request uri '{requestUri}'. Status code '{response.StatusCode.ToString()}'. Reason phrase '{response.ReasonPhrase}'. Content '{message}'.");
                }
        }
        public void Dispose()
        {
            _HttpClient?.Dispose();
        }
    }
}
