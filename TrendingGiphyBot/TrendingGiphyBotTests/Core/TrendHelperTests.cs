using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Helpers;
using TrendingGiphyBotCore.Wrappers;

namespace TrendingGiphyBotTests.Core
{
    [TestFixture]
    public class TrendHelperTests
    {
        Mock<IConfigurationWrapper> _Config;
        TrendHelper _TrendHelper;
        [SetUp]
        public void SetUp()
        {
            _Config = new Mock<IConfigurationWrapper>();
            _TrendHelper = new TrendHelper(_Config.Object);
        }
        [TestCase("8", 8, true)]
        [TestCase("22", 22, true)]
        [TestCase("24", 24, false)]
        [TestCase("32768", 0, false)]
        public void IsInRange(string quietHourString, short expectedQuietHour, bool expectedResult)
        {
            var result = _TrendHelper.IsInRange(quietHourString, out var quietHour);
            Assert.That(quietHour, Is.EqualTo(expectedQuietHour));
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(Time.Hour)]
        [TestCase(Time.Hours)]
        public void InvalidHoursConfigMessage(Time time)
        {
            var validHours = new List<short> { 1, 2, 3, 4, 6, 8, 12, 24 };
            _Config.Setup(s => s.Get<List<short>>("ValidHours")).Returns(validHours);
            var message = _TrendHelper.InvalidHoursConfigMessage(time);
            var expectedMessage = $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validHours)}.";
            Assert.That(message, Is.EqualTo(expectedMessage));
        }
        [TestCase(Time.Minute)]
        [TestCase(Time.Minutes)]
        public void InvalidMinutesConfigMessage(Time time)
        {
            var validHours = new List<short> { 10, 15, 20, 30 };
            _Config.Setup(s => s.Get<List<short>>("ValidHours")).Returns(validHours);
            var message = _TrendHelper.InvalidMinutesConfigMessage(time);
            var expectedMessage = $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validHours)}.";
            Assert.That(message, Is.EqualTo(expectedMessage));
        }
    }
}
