﻿using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class RefreshGifsFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<IGiphyWrapper> _GiphyWrapper;
        Mock<ITrendingGiphyBotContext> _Context;
        RefreshGifsFunction _RefreshGifsFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _GiphyWrapper = new Mock<IGiphyWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _RefreshGifsFunction = new RefreshGifsFunction(_Log.Object, _GiphyWrapper.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const string trendingEndpoint = "some endpoint";
            var gifObjects = new List<GifObject> { new GifObject { Id = "some ID", Url = "some url" } };
            var trendingResponse = new GiphyTrendingResponse { Data = gifObjects };
            const int count = 123;
            _GiphyWrapper.Setup(s => s.GetTrendingGifsAsync(trendingEndpoint)).ReturnsAsync(trendingResponse);
            _Context.Setup(s => s.InsertNewTrendingGifs(trendingResponse.Data)).ReturnsAsync(count);
            _Log.Setup(s => s.LogInformation($"Inserted {count} URL caches."));
            var task = _RefreshGifsFunction.RunAsync(trendingEndpoint);
            await task;
            _Log.VerifyAll();
            _GiphyWrapper.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsFaulted, Is.False);
        }
    }
}
