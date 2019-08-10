using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Functions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotTests.Functions
{
    [TestFixture]
    public class GetPrefixDictionaryFunctionTests
    {
        Mock<ITrendingGiphyBotContext> _Context;
        GetPrefixDictionaryFunction _GetPrefixDictionaryFunction;
        [SetUp]
        public void SetUp()
        {
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetPrefixDictionaryFunction = new GetPrefixDictionaryFunction(_Context.Object);
        }
        [Test]
        public async Task Run()
        {
            var log = new Mock<ILogger>();
            var prefixDictionary = new Dictionary<decimal, string> { { 123, "!" }, { 456, "^" } };
            _Context.Setup(s => s.GetPrefixDictionary()).ReturnsAsync(prefixDictionary);
            var result = await _GetPrefixDictionaryFunction.Run(null, log.Object);
            log.VerifyAll();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(prefixDictionary));
        }
    }
}
