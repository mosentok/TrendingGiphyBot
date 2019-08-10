using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class PostJobConfigFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        PostJobConfigFunction _PostJobConfigFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _PostJobConfigFunction = new PostJobConfigFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            //TODO environment vars
            //var req = new Mock<HttpRequest>();
            //var container = new JobConfigContainer();
            //var stream = new MemoryStream();
            //using (var streamWriter = new StreamWriter(stream))
            //    streamWriter.Write(container);
            //req.Setup(s => s.Body).Returns(stream);
            //var log = new Mock<ILogger>();
            //const decimal channelId = 123;
            //var updatedContainer = new JobConfigContainer();
            //_Context.Setup(s => s.SetJobConfig(channelId, container)).ReturnsAsync(updatedContainer);
            //var result = await _PostJobConfigFunction.Run(req.Object, channelId, log.Object);
            //_Context.VerifyAll();
            //var okObjectResult = result as OkObjectResult;
            //Assert.That(okObjectResult, Is.Not.Null);
            //Assert.That(okObjectResult.Value, Is.EqualTo(updatedContainer));
        }
    }
}
