using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class DeleteOldUrlHistoriesFunctionTests
    {
        Mock<ILogger> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteOldUrlHistoriesFunction _DeleteOldUrlHistoriesFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteOldUrlHistoriesFunction = new DeleteOldUrlHistoriesFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var now = DateTime.Now;
            const int count = 123;
            _Context.Setup(s => s.DeleteUrlHistoriesOlderThan(now)).ReturnsAsync(count);
            var task = _DeleteOldUrlHistoriesFunction.RunAsync(now);
            await task;
            _Context.VerifyAll();
            Assert.That(task.IsFaulted, Is.False);
        }
    }
}
