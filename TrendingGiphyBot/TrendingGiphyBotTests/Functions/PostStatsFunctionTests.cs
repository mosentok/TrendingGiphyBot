using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Exceptions;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class PostStatsFunctionTests
    {
        Mock<IStatWrapper> _StatWrapper;
        PostStatsFunction _PostStatsFunction;
        [SetUp]
        public void SetUp()
        {
            _StatWrapper = new Mock<IStatWrapper>();
            _PostStatsFunction = new PostStatsFunction(_StatWrapper.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            //TODO HttpRequest
            //const long botId = 456;
            //const string requestUri = "some/bot/url/456/stats";
            //const string content = "{\"guild_count\":123}";
            //var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            //var statPosts = new List<StatPost> { statPost };
            //var req = new Mock<HttpRequest>();
            //var stream = BuildTestStream(statPosts);
            //req.Setup(s => s.Body).Returns(stream);
            //_StatWrapper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).Returns(Task.CompletedTask);
            //var log = new Mock<ILogger>();
            //var result = await _PostStatsFunction.Run(req.Object, botId, log.Object);
            //_StatWrapper.VerifyAll();
            //Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        public async Task RunAsync_StatPostException()
        {
            //TODO HttpRequest
            //const long botId = 456;
            //const string requestUri = "some/bot/url/456/stats";
            //const string content = "{\"guild_count\":123}";
            //var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            //var statPosts = new List<StatPost> { statPost };
            //var req = new Mock<HttpRequest>();
            //var stream = BuildTestStream(statPosts);
            //req.Setup(s => s.Body).Returns(stream);
            //var exception = new StatPostException();
            //_StatWrapper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(exception);
            //var log = new Mock<ILogger>();
            //var result = await _PostStatsFunction.Run(req.Object, botId, log.Object);
            //_StatWrapper.VerifyAll();
            //Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        public void RunAsync_OtherException()
        {
            //TODO HttpRequest
            //const long botId = 456;
            //const string requestUri = "some/bot/url/456/stats";
            //const string content = "{\"guild_count\":123}";
            //var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            //var statPosts = new List<StatPost> { statPost };
            //var req = new Mock<HttpRequest>();
            //var stream = BuildTestStream(statPosts);
            //req.Setup(s => s.Body).Returns(stream);
            //_StatWrapper.Setup(s => s.PostStatAsync(requestUri, content, "bot token")).ThrowsAsync(new Exception());
            //var log = new Mock<ILogger>();
            //Assert.That(async () => await _PostStatsFunction.Run(req.Object, botId, log.Object), Throws.InstanceOf<Exception>());
            //_StatWrapper.VerifyAll();
        }
        static MemoryStream BuildTestStream(object value)
        {
            var stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream))
                streamWriter.Write(value);
            return stream;
        }
    }
}
