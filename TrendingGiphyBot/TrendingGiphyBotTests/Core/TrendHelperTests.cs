using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using TrendingGiphyBotCore.Helpers;

namespace TrendingGiphyBotTests.Core
{
    [TestFixture]
    public class TrendHelperTests
    {
        Mock<IConfiguration> _Config;
        TrendHelper _TrendHelper;
        [SetUp]
        public void SetUp()
        {
            _Config = new Mock<IConfiguration>();
            _TrendHelper = new TrendHelper(_Config.Object);
        }
        [TestCase(null, null)]
        [TestCase("apex legends", "apex legends")]
        [TestCase("gifs of apex legends", "apex legends")]
        [TestCase("gif of apex legends", "apex legends")]
        [TestCase("gifs apex legends", "apex legends")]
        [TestCase("gif apex legends", "apex legends")]
        public void CleanRandomSearchString(string randomSearchString, string expectedResult)
        {
            var result = _TrendHelper.CleanRandomSearchString(randomSearchString);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
