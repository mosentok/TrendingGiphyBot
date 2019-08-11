using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
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
            const long botId = 456;
            var statPost = new StatPost { GuildCountPropertyName = "guild_count", UrlStringFormat = "some/bot/url/{0}/stats", Token = "bot token" };
            var statPosts = new List<StatPost> { statPost };
            const int guildCount = 123;
            var container = new GuildCountContainer { GuildCount = guildCount };
            var log = new Mock<ILogger>();
            _StatWrapper.Setup(s => s.PostStatsAsync(botId,  container.GuildCount, log.Object)).Returns(Task.CompletedTask);
            var result = await _PostStatsFunction.Run(container, botId, log.Object);
            _StatWrapper.VerifyAll();
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
