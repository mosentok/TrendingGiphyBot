using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class GetPrefixDictionaryFunctionTests
    {
        Mock<ILogger> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        GetPrefixDictionaryFunction _GetPrefixDictionaryFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILogger>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetPrefixDictionaryFunction = new GetPrefixDictionaryFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            var prefixDictionary = new Dictionary<decimal, string>();
            _Context.Setup(s => s.GetPrefixDictionary()).ReturnsAsync(prefixDictionary);
            var result = await _GetPrefixDictionaryFunction.RunAsync();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(prefixDictionary));
        }
    }
}
