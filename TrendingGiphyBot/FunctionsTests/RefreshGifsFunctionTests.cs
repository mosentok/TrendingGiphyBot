using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class RefreshGifsFunctionTests
    {
        Mock<ILogger> _Log;
        Mock<IGiphyHelper> _GiphyHelper;
        Mock<ITrendingGiphyBotContext> _Context;
        RefreshGifsFunction _RefreshGifsFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _GiphyHelper = new Mock<IGiphyHelper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _RefreshGifsFunction = new RefreshGifsFunction(_Log.Object, _GiphyHelper.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const string trendingEndpoint = "some endpoint";
            var gifObjects = new List<GifObject> { new GifObject { Id = "some ID", Url = "some url" } };
            var trendingResponse = new GiphyTrendingResponse { Data = gifObjects };
            const int count = 123;
            _GiphyHelper.Setup(s => s.GetTrendingGifsAsync(trendingEndpoint)).ReturnsAsync(trendingResponse);
            _Context.Setup(s => s.InsertNewTrendingGifs(trendingResponse.Data)).ReturnsAsync(count);
            var task = _RefreshGifsFunction.RunAsync(trendingEndpoint);
            await task;
            Assert.That(task.IsFaulted, Is.False);
            _GiphyHelper.VerifyAll();
            _Context.VerifyAll();
        }
    }
}
