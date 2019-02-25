using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;

namespace FunctionsTests
{
    [TestFixture]
    public class PostStatsFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<IStatHelper> _StatHelper;
        PostStatsFunction _PostStatsFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _StatHelper = new Mock<IStatHelper>();
            _PostStatsFunction = new PostStatsFunction(_Log.Object, _StatHelper.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            _Log.Setup(s => s.LogInformation("Posting stats."));
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).Returns(Task.CompletedTask);
            _Log.Setup(s => s.LogInformation("Posted stats."));
            var result = await _PostStatsFunction.RunAsync(guildCount, botId, statPosts);
            _Log.VerifyAll();
            _StatHelper.VerifyAll();
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        public async Task RunAsync_StatPostException()
        {
            _Log.Setup(s => s.LogInformation("Posting stats."));
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            var exception = new StatPostException();
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(exception);
            _Log.Setup(s => s.LogError(exception, $"Error posting stats."));
            _Log.Setup(s => s.LogInformation("Posted stats."));
            var result = await _PostStatsFunction.RunAsync(guildCount, botId, statPosts);
            _Log.VerifyAll();
            _StatHelper.VerifyAll();
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        public void RunAsync_OtherException()
        {
            _Log.Setup(s => s.LogInformation("Posting stats."));
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(new Exception());
            Assert.That(async () => await _PostStatsFunction.RunAsync(guildCount, botId, statPosts), Throws.InstanceOf<Exception>());
            _Log.VerifyAll();
            _StatHelper.VerifyAll();
        }
    }
}
