using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class PostJobConfigFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        PostJobConfigFunction _PostJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _PostJobConfigFunction = new PostJobConfigFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const decimal channelId = 123;
            _Log.Setup(s => s.LogInformation($"Channel {channelId} posting job config."));
            var container = new JobConfigContainer();
            var updatedContainer = new JobConfigContainer();
            _Context.Setup(s => s.SetJobConfig(channelId, container)).ReturnsAsync(updatedContainer);
            _Log.Setup(s => s.LogInformation($"Channel {channelId} posted job config."));
            var result = await _PostJobConfigFunction.RunAsync(container, channelId);
            _Log.VerifyAll();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(updatedContainer));
        }
    }
}
