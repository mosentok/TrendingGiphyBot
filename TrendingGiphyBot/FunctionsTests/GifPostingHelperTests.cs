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
        Mock<IGiphyWrapper> _GiphyWrapper;
        Mock<IDiscordWrapper> _DiscordWrapper;
        GifPostingHelper _GifPostingHelper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GiphyWrapper = new Mock<IGiphyWrapper>();
            _DiscordWrapper = new Mock<IDiscordWrapper>();
            _GifPostingHelper = new GifPostingHelper(_Log.Object, _Context.Object, _GiphyWrapper.Object, _DiscordWrapper.Object);
        }
        [Test]
        public async Task LogInAsync()
        {
            _DiscordWrapper.Setup(s => s.LogInAsync()).Returns(Task.CompletedTask);
            var task = _GifPostingHelper.LogInAsync();
            await task;
            _DiscordWrapper.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task LogOutAsync()
        {
            _DiscordWrapper.Setup(s => s.LogOutAsync()).Returns(Task.CompletedTask);
            var task = _GifPostingHelper.LogOutAsync();
            await task;
            _DiscordWrapper.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
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
        [Test]
        public async Task InsertHistories()
        {
            var historyContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _Log.Setup(s => s.LogInformation($"Inserting {historyContainers.Count} histories."));
            var inserted = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _Context.Setup(s => s.InsertUrlHistories(historyContainers)).ReturnsAsync(inserted);
            _Log.Setup(s => s.LogInformation($"Inserted {inserted.Count} histories."));
            var results = await _GifPostingHelper.InsertHistories(historyContainers);
            _Log.VerifyAll();
            _Context.VerifyAll();
            foreach (var result in results)
                Assert.That(inserted, Contains.Item(result));
        }
        [Test]
        public async Task DeleteErrorHistories()
        {
            var errors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _Log.Setup(s => s.LogError($"Deleting {errors.Count} histories."));
            const int deletedCount = 123;
            _Context.Setup(s => s.DeleteUrlHistories(errors)).ReturnsAsync(deletedCount);
            _Log.Setup(s => s.LogError($"Deleted {deletedCount} histories."));
            var task = _GifPostingHelper.DeleteErrorHistories(errors);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task DeleteJobConfigs()
        {
            var channelIds = new List<decimal> { 123 };
            _Log.Setup(s => s.LogError($"Deleting {channelIds.Count} job configs."));
            const int deletedCount = 123;
            _Context.Setup(s => s.DeleteJobConfigs(channelIds)).ReturnsAsync(deletedCount);
            _Log.Setup(s => s.LogError($"Deleted {deletedCount} job configs."));
            var task = _GifPostingHelper.DeleteJobConfigs(channelIds);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
