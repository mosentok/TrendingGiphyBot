using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;

namespace FunctionsTests
{
    [TestFixture]
    public class PostStatsFunctionTests
    {
        Mock<ILogger> _Log;
        Mock<IStatHelper> _StatHelper;
        PostStatsFunction _PostStatsFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _StatHelper = new Mock<IStatHelper>();
            _PostStatsFunction = new PostStatsFunction(_Log.Object, _StatHelper.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).Returns(Task.CompletedTask);
            var result = await _PostStatsFunction.RunAsync(guildCount, botId, statPosts);
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _StatHelper.VerifyAll();
        }
        [Test]
        public async Task RunAsync_StatPostException()
        {
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(new StatPostException());
            var result = await _PostStatsFunction.RunAsync(guildCount, botId, statPosts);
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _StatHelper.VerifyAll();
        }
        [Test]
        public void RunAsync_OtherException()
        {
            const int guildCount = 123;
            const long botId = 456;
            const string requestUri = "some/bot/url/456/stats";
            const string content = "{\"guild_count\":123}";
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            _StatHelper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(new Exception());
            Assert.That(async () => await _PostStatsFunction.RunAsync(guildCount, botId, statPosts), Throws.InstanceOf<Exception>());
            _StatHelper.VerifyAll();
        }
    }
}
