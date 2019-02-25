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
    public class DeleteJobConfigFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteJobConfigFunction _DeleteJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteJobConfigFunction = new DeleteJobConfigFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const decimal channelId = 123;
            _Log.Setup(s => s.LogInformation($"Channel {channelId} deleting job config."));
            var container = new JobConfigContainer();
            _Context.Setup(s => s.DeleteJobConfig(channelId)).Returns(Task.CompletedTask);
            _Log.Setup(s => s.LogInformation($"Channel {channelId} deleted job config."));
            var result = await _DeleteJobConfigFunction.RunAsync(channelId);
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}
