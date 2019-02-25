using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class GifPostingHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        Mock<IGiphyHelper> _GiphyHelper;
        Mock<IDiscordHelper> _DiscordHelper;
        GifPostingHelper _GifPostingHelper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GiphyHelper = new Mock<IGiphyHelper>();
            _DiscordHelper = new Mock<IDiscordHelper>();
            _GifPostingHelper = new GifPostingHelper(_Log.Object, _Context.Object, _GiphyHelper.Object, _DiscordHelper.Object);
        }
        [TestCase(0, 0, 1440)]
        [TestCase(0, 10, 10)]
        [TestCase(0, 15, 15)]
        [TestCase(0, 20, 20)]
        [TestCase(0, 30, 30)]
        [TestCase(0, 40, 40)]
        [TestCase(0, 45, 45)]
        [TestCase(0, 50, 50)]
        [TestCase(1, 0, 60)]
        [TestCase(1, 10, 70)]
        [TestCase(1, 15, 75)]
        [TestCase(1, 20, 80)]
        [TestCase(1, 30, 90)]
        [TestCase(1, 40, 100)]
        [TestCase(1, 45, 105)]
        [TestCase(1, 50, 110)]
        [TestCase(2, 0, 120)]
        public void DetermineTotalMinutes(int nowHour, int nowMinutes, int expectedTotalMinutes)
        {
            var now = new DateTime(1988, 2, 12, nowHour, nowMinutes, 0);
            var totalMinutes = _GifPostingHelper.DetermineTotalMinutes(now);
            Assert.That(totalMinutes, Is.EqualTo(expectedTotalMinutes));
        }
        [TestCase(10, 10)]
        [TestCase(15, 15)]
        [TestCase(20, 10, 20)]
        [TestCase(30, 10, 15, 30)]
        [TestCase(40, 10, 20)]
        [TestCase(45, 15)]
        [TestCase(50, 10)]
        [TestCase(60, 10, 15, 20, 30, 60)]
        [TestCase(1440, 10, 15, 20, 30, 60, 120, 180, 240, 360, 480, 720, 1440)]
        public void DetermineCurrentValidMinutes(int totalMinutes, params int[] expectedValidMinutes)
        {
            var allValidMinutes = new List<int> { 10, 15, 20, 30, 60, 120, 180, 240, 360, 480, 720, 1440 };
            var currentValidMinutes = _GifPostingHelper.DetermineCurrentValidMinutes(totalMinutes, allValidMinutes);
            Assert.That(currentValidMinutes.Count, Is.EqualTo(expectedValidMinutes.Length));
            foreach (var expectedValidMinute in expectedValidMinutes)
                Assert.That(currentValidMinutes, Contains.Item(expectedValidMinute));
        }
        [Test]
        public async Task GetContainers()
        {
            _Log.Setup(s => s.LogInformation("Getting job configs."));
            const int nowHour = 1;
            var currentValidMinutes = new List<int> { 10, 15, 20, 30, 60 };
            var pendingJobConfig = new PendingJobConfig { ChannelId = 123, Histories = new List<PendingHistory>(), RandomSearchString = "cats" };
            var containers = new List<PendingJobConfig> { pendingJobConfig };
            _Context.Setup(s => s.GetJobConfigsToRun(nowHour, currentValidMinutes)).ReturnsAsync(containers);
            _Log.Setup(s => s.LogInformation($"Got {containers.Count} containers."));
            var jobConfigs = await _GifPostingHelper.GetContainers(nowHour, currentValidMinutes);
            _Log.VerifyAll();
            _Context.VerifyAll();
            foreach (var container in containers)
                Assert.That(jobConfigs, Contains.Item(container));
        }
    }
}
