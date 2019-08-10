using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class DeleteOldUrlCachesFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteOldUrlCachesFunction _DeleteOldUrlCachesFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteOldUrlCachesFunction = new DeleteOldUrlCachesFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            var oldestDate = DateTime.Now.AddDays(-7);
            _Context.Setup(s => s.GetUrlCachesOldestDate()).Returns(oldestDate);
            _Context.Setup(s => s.DeleteUrlCachesOlderThan(oldestDate)).ReturnsAsync(123);
            var task = _DeleteOldUrlCachesFunction.Run(null, log.Object);
            await task;
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
