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
    public class DeleteOldUrlHistoriesFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        DeleteOldUrlHistoriesFunction _DeleteOldUrlHistoriesFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _DeleteOldUrlHistoriesFunction = new DeleteOldUrlHistoriesFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            //TODO environment vars
            //var log = new Mock<ILogger>();
            //var oldestDate = DateTime.Now;
            //const int count = 123;
            //_Context.Setup(s => s.DeleteUrlHistoriesOlderThan(oldestDate)).ReturnsAsync(count);
            //var task = _DeleteOldUrlHistoriesFunction.Run(null, log.Object);
            //await task;
            //log.VerifyAll();
            //_Context.VerifyAll();
            //Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
