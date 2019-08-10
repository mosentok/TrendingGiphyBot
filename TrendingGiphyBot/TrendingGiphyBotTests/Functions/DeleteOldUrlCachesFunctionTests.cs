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
            //TODO
            //var log = new Mock<ILogger>();
            //const int urlCachesMaxDaysOld = 7;
            //const int count = 123;
            //var oldestDate = DateTime.Now.AddDays(-urlCachesMaxDaysOld);
            //Environment.SetEnvironmentVariable("UrlCachesMaxDaysOld", urlCachesMaxDaysOld.ToString());
            //_Context.Setup(s => s.DeleteUrlCachesOlderThan(oldestDate)).ReturnsAsync(count);
            //var task = _DeleteOldUrlCachesFunction.Run(null, log.Object);
            //await task;
            //log.VerifyAll();
            //_Context.VerifyAll();
            //Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
