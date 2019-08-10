using Discord;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class PostGifsHelperTests
    {
        Mock<ILogger> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        Mock<IGiphyWrapper> _GiphyWrapper;
        Mock<IDiscordWrapper> _DiscordWrapper;
        PostGifsHelper _PostGifsHelper;
        static readonly List<string> warningResponses = new List<string> { "missing access", "missing permission" };
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GiphyWrapper = new Mock<IGiphyWrapper>();
            _DiscordWrapper = new Mock<IDiscordWrapper>();
            var services = new ServiceCollection()
                .AddSingleton(_Context.Object)
                .AddSingleton(_GiphyWrapper.Object)
                .AddSingleton(_DiscordWrapper.Object);
            _PostGifsHelper = new PostGifsHelper(services.BuildServiceProvider(), warningResponses);
        }
        [Test]
        public async Task LogInAsync()
        {
            const string botToken = "atoken";
            _DiscordWrapper.Setup(s => s.LogInAsync(botToken)).Returns(Task.CompletedTask);
            var task = _PostGifsHelper.LogInAsync(botToken);
            await task;
            _DiscordWrapper.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task LogOutAsync()
        {
            _DiscordWrapper.Setup(s => s.LogOutAsync()).Returns(Task.CompletedTask);
            var task = _PostGifsHelper.LogOutAsync();
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
            var totalMinutes = _PostGifsHelper.DetermineTotalMinutes(now);
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
            var currentValidMinutes = _PostGifsHelper.DetermineCurrentValidMinutes(totalMinutes, allValidMinutes);
            Assert.That(currentValidMinutes.Count, Is.EqualTo(expectedValidMinutes.Length));
            foreach (var expectedValidMinute in expectedValidMinutes)
                Assert.That(currentValidMinutes, Contains.Item(expectedValidMinute));
        }
        [Test]
        public async Task GetContainers()
        {
            const int nowHour = 1;
            var currentValidMinutes = new List<int> { 10, 15, 20, 30, 60 };
            var pendingJobConfig = new PendingJobConfig { ChannelId = 123, Histories = new List<PendingHistory>(), RandomSearchString = "cats" };
            var containers = new List<PendingJobConfig> { pendingJobConfig };
            _Context.Setup(s => s.GetJobConfigsToRun(nowHour, currentValidMinutes)).ReturnsAsync(containers);
            var jobConfigs = await _PostGifsHelper.GetContainers(nowHour, currentValidMinutes, _Log.Object);
            _Context.VerifyAll();
            foreach (var container in containers)
                Assert.That(jobConfigs, Contains.Item(container));
        }
        [Test]
        public async Task InsertHistories()
        {
            var historyContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var inserted = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _Context.Setup(s => s.InsertUrlHistories(historyContainers)).ReturnsAsync(inserted);
            var results = await _PostGifsHelper.InsertHistories(historyContainers, _Log.Object);
            _Context.VerifyAll();
            foreach (var result in results)
                Assert.That(inserted, Contains.Item(result));
        }
        [Test]
        public async Task DeleteErrorHistories()
        {
            var errors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            const int deletedCount = 123;
            _Context.Setup(s => s.DeleteUrlHistories(errors)).ReturnsAsync(deletedCount);
            var task = _PostGifsHelper.DeleteErrorHistories(errors, _Log.Object);
            await task;
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task DeleteJobConfigs()
        {
            var channelIds = new List<decimal> { 123 };
            const int deletedCount = 123;
            _Context.Setup(s => s.DeleteJobConfigs(channelIds)).ReturnsAsync(deletedCount);
            var task = _PostGifsHelper.DeleteJobConfigs(channelIds, _Log.Object);
            await task;
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task BuildHistoryContainers_UnseenCache()
        {
            const string alreadySeenGifId = "already seen";
            const string notYetSeenGifId = "not yet seen";
            var histories = new List<PendingHistory>
            {
                new PendingHistory { GifId = alreadySeenGifId }
            };
            var containers = new List<PendingJobConfig> { new PendingJobConfig { Histories = histories } };
            var urlCaches = new List<UrlCache> { new UrlCache { Id = alreadySeenGifId }, new UrlCache { Id = notYetSeenGifId } };
            _Context.Setup(s => s.GetUrlCachesAsync()).ReturnsAsync(urlCaches);
            const string giphyRandomEndpoint = "giphy random endpoint";
            var result = await _PostGifsHelper.BuildHistoryContainers(containers, giphyRandomEndpoint, _Log.Object);
            _Context.VerifyAll();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Count(s => s.GifId == alreadySeenGifId), Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == notYetSeenGifId), Is.EqualTo(1));
        }
        [Test]
        public async Task BuildHistoryContainers_RandomGifNotYetSeen()
        {
            const string alreadySeenGifId = "already seen";
            const string anotherSeenGifId = "another seen";
            const string notYetSeenGifId = "not yet seen";
            var histories = new List<PendingHistory>
            {
                new PendingHistory { GifId = alreadySeenGifId },
                new PendingHistory { GifId = anotherSeenGifId }
            };
            var pendingJobConfig = new PendingJobConfig { Histories = histories, RandomSearchString = "apex legends" };
            var containers = new List<PendingJobConfig> { pendingJobConfig };
            var urlCaches = new List<UrlCache> { new UrlCache { Id = alreadySeenGifId }, new UrlCache { Id = anotherSeenGifId } };
            _Context.Setup(s => s.GetUrlCachesAsync()).ReturnsAsync(urlCaches);
            const string giphyRandomEndpoint = "giphy random endpoint";
            var giphyRandomResponse = new GiphyRandomResponse { Data = new GifObject { Id = notYetSeenGifId } };
            _GiphyWrapper.Setup(s => s.GetRandomGifAsync(giphyRandomEndpoint, pendingJobConfig.RandomSearchString)).ReturnsAsync(giphyRandomResponse);
            var result = await _PostGifsHelper.BuildHistoryContainers(containers, giphyRandomEndpoint, _Log.Object);
            _Context.VerifyAll();
            _GiphyWrapper.VerifyAll();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Count(s => s.GifId == alreadySeenGifId), Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == anotherSeenGifId), Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == notYetSeenGifId), Is.EqualTo(1));
        }
        [Test]
        public async Task BuildHistoryContainers_RandomGifAlreadySeen()
        {
            const string alreadySeenGifId = "already seen";
            const string anotherSeenGifId = "another seen";
            const string randomGifAlreadySeenId = "random gif already seen";
            var histories = new List<PendingHistory>
            {
                new PendingHistory { GifId = alreadySeenGifId },
                new PendingHistory { GifId = anotherSeenGifId },
                new PendingHistory { GifId = randomGifAlreadySeenId }
            };
            var pendingJobConfig = new PendingJobConfig { Histories = histories, RandomSearchString = "apex legends" };
            var containers = new List<PendingJobConfig> { pendingJobConfig };
            var urlCaches = new List<UrlCache> { new UrlCache { Id = alreadySeenGifId }, new UrlCache { Id = anotherSeenGifId } };
            _Context.Setup(s => s.GetUrlCachesAsync()).ReturnsAsync(urlCaches);
            const string giphyRandomEndpoint = "giphy random endpoint";
            var giphyRandomResponse = new GiphyRandomResponse { Data = new GifObject { Id = randomGifAlreadySeenId } };
            _GiphyWrapper.Setup(s => s.GetRandomGifAsync(giphyRandomEndpoint, pendingJobConfig.RandomSearchString)).ReturnsAsync(giphyRandomResponse);
            var result = await _PostGifsHelper.BuildHistoryContainers(containers, giphyRandomEndpoint, _Log.Object);
            _Context.VerifyAll();
            _GiphyWrapper.VerifyAll();
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == alreadySeenGifId), Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == anotherSeenGifId), Is.EqualTo(0));
            Assert.That(result.Count(s => s.GifId == randomGifAlreadySeenId), Is.EqualTo(0));
        }
        [Test]
        public async Task BuildChannelContainers()
        {
            const decimal channelId = 123;
            var insertedContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer { ChannelId = channelId } };
            var channel = new Mock<IMessageChannel>();
            var channelIdLong = Convert.ToUInt64(channelId);
            channel.Setup(s => s.Id).Returns(channelIdLong);
            _DiscordWrapper.Setup(s => s.GetChannelAsync(channelId)).ReturnsAsync(channel.Object);
            var result = await _PostGifsHelper.BuildChannelContainers(insertedContainers, _Log.Object);
            _DiscordWrapper.VerifyAll();
            Assert.That(result.ChannelContainers.Count, Is.EqualTo(1));
            Assert.That(result.Errors.Count, Is.EqualTo(0));
            Assert.That(result.ChannelsToDelete.Count, Is.EqualTo(0));
            Assert.That(result.ChannelContainers.Count(s => s.Channel.Id == channelIdLong), Is.EqualTo(1));
        }
        [Test]
        public async Task BuildChannelContainers_Error()
        {
            const decimal channelId = 123;
            var errorHistory = new UrlHistoryContainer { ChannelId = channelId };
            var insertedContainers = new List<UrlHistoryContainer> { errorHistory };
            var channel = new Mock<IMessageChannel>();
            var channelIdLong = Convert.ToUInt64(channelId);
            channel.Setup(s => s.Id).Returns(channelIdLong);
            var exception = new Exception();
            _DiscordWrapper.Setup(s => s.GetChannelAsync(channelId)).ThrowsAsync(exception);
            var result = await _PostGifsHelper.BuildChannelContainers(insertedContainers, _Log.Object);
            _DiscordWrapper.VerifyAll();
            Assert.That(result.ChannelContainers.Count, Is.EqualTo(0));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.ChannelsToDelete.Count, Is.EqualTo(0));
            Assert.That(result.Errors, Contains.Item(errorHistory));
        }
        [Test]
        public async Task BuildChannelContainers_NullChannel()
        {
            const decimal channelId = 123;
            var errorHistory = new UrlHistoryContainer { ChannelId = channelId };
            var insertedContainers = new List<UrlHistoryContainer> { errorHistory };
            IMessageChannel channel = null;
            var channelIdLong = Convert.ToUInt64(channelId);
            _DiscordWrapper.Setup(s => s.GetChannelAsync(channelId)).ReturnsAsync(channel);
            var result = await _PostGifsHelper.BuildChannelContainers(insertedContainers, _Log.Object);
            _DiscordWrapper.VerifyAll();
            Assert.That(result.ChannelContainers.Count, Is.EqualTo(0));
            Assert.That(result.Errors.Count, Is.EqualTo(0));
            Assert.That(result.ChannelsToDelete.Count, Is.EqualTo(1));
            Assert.That(result.ChannelsToDelete, Contains.Item(channelId));
        }
        [Test]
        public async Task PostGifs_Trending()
        {
            var channel = new Mock<IMessageChannel>();
            var historyContainer = new UrlHistoryContainer { IsTrending = true, Url = "a.b/c" };
            IUserMessage userMessage = null;
            var message = $"*Trending!* {historyContainer.Url}";
            channel.Setup(s => s.SendMessageAsync(message, It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>())).ReturnsAsync(userMessage);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer { Channel = channel.Object, HistoryContainer = historyContainer } };
            var result = await _PostGifsHelper.PostGifs(channelContainers, _Log.Object);
            channel.VerifyAll();
            Assert.That(result.ChannelsToDelete, Is.Empty);
            Assert.That(result.Errors, Is.Empty);
        }
        [Test]
        public async Task PostGifs_RandomGif()
        {
            var channel = new Mock<IMessageChannel>();
            var historyContainer = new UrlHistoryContainer { IsTrending = false, Url = "a.b/c" };
            IUserMessage userMessage = null;
            channel.Setup(s => s.SendMessageAsync(historyContainer.Url, It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>())).ReturnsAsync(userMessage);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer { Channel = channel.Object, HistoryContainer = historyContainer } };
            var result = await _PostGifsHelper.PostGifs(channelContainers, _Log.Object);
            channel.VerifyAll();
            Assert.That(result.ChannelsToDelete, Is.Empty);
            Assert.That(result.Errors, Is.Empty);
        }
        [Test]
        public async Task PostGifs_HttpException()
        {
            var channel = new Mock<IMessageChannel>();
            const decimal channelId = 123;
            var historyContainer = new UrlHistoryContainer { ChannelId = channelId, IsTrending = false, Url = "a.b/c" };
            var request = new Mock<IRequest>();
            var httpException = new HttpException(HttpStatusCode.Unauthorized, request.Object, 50001, "missing access");
            channel.Setup(s => s.SendMessageAsync(historyContainer.Url, It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>())).ThrowsAsync(httpException);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer { Channel = channel.Object, HistoryContainer = historyContainer } };
            var result = await _PostGifsHelper.PostGifs(channelContainers, _Log.Object);
            channel.VerifyAll();
            Assert.That(result.ChannelsToDelete, Contains.Item(channelId));
            Assert.That(result.Errors, Is.Empty);
        }
        [Test]
        public async Task PostGifs_Exception()
        {
            var channel = new Mock<IMessageChannel>();
            const decimal channelId = 123;
            var historyContainer = new UrlHistoryContainer { ChannelId = channelId, IsTrending = false, GifId = "the gif ID", Url = "a.b/c" };
            var request = new Mock<IRequest>();
            var exception = new Exception();
            channel.Setup(s => s.SendMessageAsync(historyContainer.Url, It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>())).ThrowsAsync(exception);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer { Channel = channel.Object, HistoryContainer = historyContainer } };
            var result = await _PostGifsHelper.PostGifs(channelContainers, _Log.Object);
            channel.VerifyAll();
            Assert.That(result.ChannelsToDelete, Is.Empty);
            Assert.That(result.Errors, Contains.Item(historyContainer));
        }
    }
}
