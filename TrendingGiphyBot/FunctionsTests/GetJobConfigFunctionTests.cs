﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class GetJobConfigFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        GetJobConfigFunction _GetJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetJobConfigFunction = new GetJobConfigFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const decimal channelId = 123;
            _Log.Setup(s => s.LogInformation($"Channel {channelId} getting job config."));
            var container = new JobConfigContainer();
            _Context.Setup(s => s.GetJobConfig(channelId)).ReturnsAsync(container);
            _Log.Setup(s => s.LogInformation($"Channel {channelId} got job config."));
            var result = await _GetJobConfigFunction.RunAsync(channelId);
            _Log.VerifyAll();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(container));
        }
    }
}
