using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace FunctionsTests
{
    [TestFixture]
    public class GetPrefixDictionaryFunctionTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITrendingGiphyBotContext> _Context;
        GetPrefixDictionaryFunction _GetPrefixDictionaryFunction;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITrendingGiphyBotContext>();
            _GetPrefixDictionaryFunction = new GetPrefixDictionaryFunction(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            _Log.Setup(s => s.LogInformation("Getting prefix dictionary."));
            var prefixDictionary = new Dictionary<decimal, string> { { 123, "!" }, { 456, "^" } };
            _Context.Setup(s => s.GetPrefixDictionary()).ReturnsAsync(prefixDictionary);
            _Log.Setup(s => s.LogInformation($"Got {prefixDictionary.Count} prefixes."));
            var result = await _GetPrefixDictionaryFunction.RunAsync();
            _Log.VerifyAll();
            _Context.VerifyAll();
            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult, Is.Not.Null);
            Assert.That(okObjectResult.Value, Is.EqualTo(prefixDictionary));
        }
    }
}
