using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class DeleteOldUrlCachesHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteOldUrlCachesHelper _DeleteOldUrlCachesHelper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteOldUrlCachesHelper = new DeleteOldUrlCachesHelper(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var oldestDate = DateTime.Now;
            _Log.Setup(s => s.LogInformation($"Deleting URL caches older than {oldestDate}."));
            const int count = 123;
            _Context.Setup(s => s.DeleteUrlCachesOlderThan(oldestDate)).ReturnsAsync(count);
            _Log.Setup(s => s.LogInformation($"Deleted {count} URL caches older than {oldestDate}."));
            var task = _DeleteOldUrlCachesHelper.RunAsync(oldestDate);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
