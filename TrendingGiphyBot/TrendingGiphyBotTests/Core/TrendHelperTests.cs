using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TrendingGiphyBotCore.Configuration;
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
            var validMinutes = new List<short> { 10, 15, 20, 30 };
            _Config.Setup(s => s.Get<List<short>>("ValidMinutes")).Returns(validMinutes);
            var message = _TrendHelper.InvalidMinutesConfigMessage(time);
            var expectedMessage = $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validMinutes)}.";
            Assert.That(message, Is.EqualTo(expectedMessage));
        }
        [TestCase(10, Time.Minutes, 24, Time.Hours, "Interval must be between 10 Minutes and 24 Hours.")]
        [TestCase(123, Time.Minute, 456, Time.Hour, "Interval must be between 123 Minute and 456 Hour.")]
        public void InvalidConfigRangeMessage(short minInterval, Time minTime, short maxInterval, Time maxTime, string expectedMessage)
        {
            var minJobConfig = new SubJobConfig(minInterval, minTime);
            var maxJobConfig = new SubJobConfig(maxInterval, maxTime);
            _Config.Setup(s => s.Get<SubJobConfig>("MinJobConfig")).Returns(minJobConfig);
            _Config.Setup(s => s.Get<SubJobConfig>("MaxJobConfig")).Returns(maxJobConfig);
            var message = _TrendHelper.InvalidConfigRangeMessage();
            Assert.That(message, Is.EqualTo(expectedMessage));
        }
        [TestCase("Off", true)]
        [TestCase("asdf", false)]
        [TestCase(null, false)]
        public void ShouldTurnCommandOff(string word, bool expectedResult)
        {
            var result = _TrendHelper.ShouldTurnCommandOff(word);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
