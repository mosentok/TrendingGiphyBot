using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class GetJobConfigFunctionTests
    {
        Mock<ILogger> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        GetJobConfigFunction _GetJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetJobConfigFunction = new GetJobConfigFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const decimal channelId = 123;
            var container = new JobConfigContainer();
            _Context.Setup(s => s.GetJobConfig(channelId)).ReturnsAsync(container);
            var result = await _GetJobConfigFunction.RunAsync(channelId);
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(container));
        }
    }
}
