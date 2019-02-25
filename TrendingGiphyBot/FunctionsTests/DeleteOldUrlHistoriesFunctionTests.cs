using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class DeleteOldUrlHistoriesFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteOldUrlHistoriesFunction _DeleteOldUrlHistoriesFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteOldUrlHistoriesFunction = new DeleteOldUrlHistoriesFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var oldestDate = DateTime.Now;
            _Log.Setup(s => s.LogInformation($"Deleting URL histories older than {oldestDate}."));
            const int count = 123;
            _Context.Setup(s => s.DeleteUrlHistoriesOlderThan(oldestDate)).ReturnsAsync(count);
            _Log.Setup(s => s.LogInformation($"Deleted {count} URL histories older than {oldestDate}."));
            var task = _DeleteOldUrlHistoriesFunction.RunAsync(oldestDate);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsFaulted, Is.False);
        }
    }
}
