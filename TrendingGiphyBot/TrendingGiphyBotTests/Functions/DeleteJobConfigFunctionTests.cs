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
    public class DeleteJobConfigFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteJobConfigFunction _DeleteJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteJobConfigFunction = new DeleteJobConfigFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            const decimal channelId = 123;
            var container = new JobConfigContainer();
            _Context.Setup(s => s.DeleteJobConfig(channelId)).Returns(Task.CompletedTask);
            var result = await _DeleteJobConfigFunction.Run(null, channelId, log.Object);
            log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
