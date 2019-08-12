using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class PostJobConfigFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        PostJobConfigFunction _PostJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _PostJobConfigFunction = new PostJobConfigFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            const decimal channelId = 123;
            var container = new JobConfigContainer();
            _Context.Setup(s => s.SetJobConfig(channelId, container)).ReturnsAsync(container);
            var result = await _PostJobConfigFunction.Run(container, channelId, log.Object);
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(container));
        }
    }
}
