using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class PostGifsHelperTests
    {
        Mock<IGifPostingHelper> _GifPostingHelper;
        PostGifsHelper _PostGifsHelper;
        [SetUp]
        public void SetUp()
        {
            _GifPostingHelper = new Mock<IGifPostingHelper>();
            _PostGifsHelper = new PostGifsHelper(_GifPostingHelper.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var now = new DateTime();
            var allValidMinutes = new List<int> { 123 };
            const string giphyRandomEndpoint = "giphy random endpoint";
            var warningResponses = new List<string> { "missing access", "missing permission" };
            const int totalMinutes = 789;
            _GifPostingHelper.Setup(s => s.DetermineTotalMinutes(now)).Returns(totalMinutes);
            var currentValidMinutes = new List<int> { 456 };
            _GifPostingHelper.Setup(s => s.DetermineCurrentValidMinutes(totalMinutes, allValidMinutes)).Returns(currentValidMinutes);
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
            var task = _PostGifsHelper.RunAsync(now, allValidMinutes, giphyRandomEndpoint, warningResponses);
            await task;
            _GifPostingHelper.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
