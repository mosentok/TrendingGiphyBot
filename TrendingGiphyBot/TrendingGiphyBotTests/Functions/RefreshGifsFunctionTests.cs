using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class RefreshGifsFunctionTests
    {
        Mock<IGiphyWrapper> _GiphyWrapper;
        Mock<ITrendingGiphyBotContext> _Context;
        RefreshGifsFunction _RefreshGifsFunction;
        [SetUp]
        public void SetUp()
        {
            _GiphyWrapper = new Mock<IGiphyWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _RefreshGifsFunction = new RefreshGifsFunction(_Context.Object, _GiphyWrapper.Object);
        }
        [Test]
        public async Task Run()
        {
            var gifObjects = new List<GifObject> { new GifObject { Id = "some ID", Url = "some url" } };
            var trendingResponse = new GiphyTrendingResponse { Data = gifObjects };
            const int count = 123;
            _GiphyWrapper.Setup(s => s.GetTrendingGifsAsync()).ReturnsAsync(trendingResponse);
            _Context.Setup(s => s.InsertNewTrendingGifs(trendingResponse.Data)).ReturnsAsync(count);
            var log = new Mock<ILogger>();
            var task = _RefreshGifsFunction.Run(null, log.Object);
            await task;
            _GiphyWrapper.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
