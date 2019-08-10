﻿using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class PostGifsFunctionTests
    {
        Mock<IPostGifsHelper> _PostGifsHelper;
        PostGifsFunction _PostGifsFunction;
        [SetUp]
        public void SetUp()
        {
            _PostGifsHelper = new Mock<IPostGifsHelper>();
            _PostGifsFunction = new PostGifsFunction(_PostGifsHelper.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            _PostGifsHelper.Setup(s => s.LogInAsync()).Returns(Task.CompletedTask);
            var pendingContainers = new List<PendingJobConfig> { new PendingJobConfig() };
            _PostGifsHelper.Setup(s => s.GetContainers(log.Object)).ReturnsAsync(pendingContainers);
            var historyContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _PostGifsHelper.Setup(s => s.BuildHistoryContainers(pendingContainers, log.Object)).ReturnsAsync(historyContainers);
            var insertedContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _PostGifsHelper.Setup(s => s.InsertHistories(historyContainers, log.Object)).ReturnsAsync(insertedContainers);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer() };
            var channelResultErrors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var channelsToDeleteThatWereNull = new List<decimal> { 111 };
            var channelResult = new ChannelResult(channelContainers, channelResultErrors, channelsToDeleteThatWereNull);
            _PostGifsHelper.Setup(s => s.BuildChannelContainers(insertedContainers, log.Object)).ReturnsAsync(channelResult);
            var errors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var channelsToDeleteFromGifPosting = new List<decimal> { 777 };
            var gifPostingResult = new GifPostingResult(errors, channelsToDeleteFromGifPosting);
            _PostGifsHelper.Setup(s => s.PostGifs(channelResult.ChannelContainers, log.Object)).ReturnsAsync(gifPostingResult);
            _PostGifsHelper.Setup(s => s.DeleteErrorHistories(channelResult.Errors, log.Object)).Returns(Task.CompletedTask);
            _PostGifsHelper.Setup(s => s.DeleteJobConfigs(channelResult.ChannelsToDelete, log.Object)).Returns(Task.CompletedTask);
            _PostGifsHelper.Setup(s => s.DeleteErrorHistories(gifPostingResult.Errors, log.Object)).Returns(Task.CompletedTask);
            _PostGifsHelper.Setup(s => s.DeleteJobConfigs(gifPostingResult.ChannelsToDelete, log.Object)).Returns(Task.CompletedTask);
            var task = _PostGifsFunction.Run(null, log.Object);
            await task;
            _PostGifsHelper.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
