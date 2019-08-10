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
    public class GetJobConfigFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        GetJobConfigFunction _GetJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetJobConfigFunction = new GetJobConfigFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            const decimal channelId = 123;
            var container = new JobConfigContainer();
            _Context.Setup(s => s.GetJobConfig(channelId)).ReturnsAsync(container);
            var result = await _GetJobConfigFunction.Run(null, channelId, log.Object);
            log.VerifyAll();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(container));
        }
    }
}
