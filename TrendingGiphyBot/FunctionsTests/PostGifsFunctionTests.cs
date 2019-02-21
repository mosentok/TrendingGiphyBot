using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class PostGifsFunctionTests
    {
        Mock<IGifPostingHelper> _GifPostingHelper;
        PostGifsFunction _PostGifsFunction;
        [SetUp]
        public void SetUp()
        {
            _GifPostingHelper = new Mock<IGifPostingHelper>();
            _PostGifsFunction = new PostGifsFunction(_GifPostingHelper.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var now = new DateTime();
            var allValidMinutes = new List<int> { 123 };
            const string giphyRandomEndpoint = "giphy random endpoint";
            var warningResponses = new List<string> { "missing access", "missing permission" };
            var currentValidMinutes = new List<int> { 456 };
            _GifPostingHelper.Setup(s => s.DetermineCurrentValidMinutes(now, allValidMinutes)).Returns(currentValidMinutes);
            var pendingContainers = new List<PendingJobConfig> { new PendingJobConfig() };
            _GifPostingHelper.Setup(s => s.GetContainers(now.Hour, currentValidMinutes)).ReturnsAsync(pendingContainers);
            var historyContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _GifPostingHelper.Setup(s => s.BuildHistoryContainers(pendingContainers, giphyRandomEndpoint)).ReturnsAsync(historyContainers);
            var insertedContainers = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            _GifPostingHelper.Setup(s => s.InsertHistories(historyContainers)).ReturnsAsync(insertedContainers);
            var channelContainers = new List<ChannelContainer> { new ChannelContainer() };
            var channelResultErrors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var channelResult = new ChannelResult(channelContainers, channelResultErrors);
            _GifPostingHelper.Setup(s => s.BuildChannelContainers(insertedContainers)).ReturnsAsync(channelResult);
            var errors = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var channelsToDelete = new List<UrlHistoryContainer> { new UrlHistoryContainer() };
            var gifPostingResult = new GifPostingResult(errors, channelsToDelete);
            _GifPostingHelper.Setup(s => s.PostGifs(channelResult.ChannelContainers, warningResponses)).ReturnsAsync(gifPostingResult);
            _GifPostingHelper.Setup(s => s.DeleteErrorHistories(channelResult.Errors)).Returns(Task.CompletedTask);
            _GifPostingHelper.Setup(s => s.DeleteErrorHistories(gifPostingResult.Errors)).Returns(Task.CompletedTask);
            _GifPostingHelper.Setup(s => s.DeleteJobConfigs(gifPostingResult.ChannelsToDelete)).Returns(Task.CompletedTask);
            var task = _PostGifsFunction.RunAsync(now, allValidMinutes, giphyRandomEndpoint, warningResponses);
            await task;
            _GifPostingHelper.VerifyAll();
            Assert.That(task.IsFaulted, Is.False);
        }
    }
}
