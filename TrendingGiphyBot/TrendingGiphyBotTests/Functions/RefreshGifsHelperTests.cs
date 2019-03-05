using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class RefreshGifsHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<IGiphyWrapper> _GiphyWrapper;
        Mock<ITrendingGiphyBotContext> _Context;
        RefreshGifsHelper _RefreshGifsHelper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _GiphyWrapper = new Mock<IGiphyWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _RefreshGifsHelper = new RefreshGifsHelper(_Log.Object, _GiphyWrapper.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const string trendingEndpoint = "some endpoint";
            var gifObjects = new List<GifObject> { new GifObject { Id = "some ID", Url = "some url" } };
            var trendingResponse = new GiphyTrendingResponse { Data = gifObjects };
            const int count = 123;
            _Log.Setup(s => s.LogInformation("Getting trending gifs."));
            _GiphyWrapper.Setup(s => s.GetTrendingGifsAsync(trendingEndpoint)).ReturnsAsync(trendingResponse);
            _Log.Setup(s => s.LogInformation($"Got {trendingResponse.Data.Count} trending gifs."));
            _Context.Setup(s => s.InsertNewTrendingGifs(trendingResponse.Data)).ReturnsAsync(count);
            _Log.Setup(s => s.LogInformation($"Inserting {trendingResponse.Data.Count} URL caches."));
            _Log.Setup(s => s.LogInformation($"Inserted {count} URL caches."));
            var task = _RefreshGifsHelper.RunAsync(trendingEndpoint);
            await task;
            _Log.VerifyAll();
            _GiphyWrapper.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
