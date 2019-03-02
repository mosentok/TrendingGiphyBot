using Discord.Commands;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Helpers;
using TrendingGiphyBotCore.Wrappers;
using TrendingGiphyBotModel;

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
        [TestCase("Off", true)]
        [TestCase("asdf", false)]
        [TestCase(null, false)]
        public void ShouldTurnCommandOff(string word, bool expectedResult)
        {
            var result = _TrendHelper.ShouldTurnCommandOff(word);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(10, Time.Minutes, 24, Time.Hours, 5, Time.Minutes, "Interval must be between 10 Minutes and 24 Hours.")]
        [TestCase(123, Time.Minute, 456, Time.Hour, 5, Time.Minutes, "Interval must be between 123 Minute and 456 Hour.")]
        public void DetermineErrorMessage_Invalid(short minInterval, Time minTime, short maxInterval, Time maxTime, short desiredInterval, Time desiredTime, string expectedResult)
        {
            var minJobConfig = new SubJobConfig(minInterval, minTime);
            var maxJobConfig = new SubJobConfig(maxInterval, maxTime);
            _Config.Setup(s => s.Get<SubJobConfig>("MinJobConfig")).Returns(minJobConfig);
            _Config.Setup(s => s.Get<SubJobConfig>("MaxJobConfig")).Returns(maxJobConfig);
            var result = _TrendHelper.DetermineErrorMessage(desiredInterval, desiredTime);
            _Config.VerifyAll();
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(10, Time.Minutes, 24, Time.Hours, 10, Time.Minutes, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 15, Time.Minutes, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 20, Time.Minutes, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 30, Time.Minutes, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 12, Time.Minutes, "When Time is Minutes, interval must be 10, 15, 20, 30.")]
        public void DetermineErrorMessage_Minutes(short minInterval, Time minTime, short maxInterval, Time maxTime, short desiredInterval, Time desiredTime, string expectedResult)
        {
            var validMinutes = new List<short> { 10, 15, 20, 30 };
            _Config.Setup(s => s.Get<List<short>>("ValidMinutes")).Returns(validMinutes);
            var minJobConfig = new SubJobConfig(minInterval, minTime);
            var maxJobConfig = new SubJobConfig(maxInterval, maxTime);
            _Config.Setup(s => s.Get<SubJobConfig>("MinJobConfig")).Returns(minJobConfig);
            _Config.Setup(s => s.Get<SubJobConfig>("MaxJobConfig")).Returns(maxJobConfig);
            var result = _TrendHelper.DetermineErrorMessage(desiredInterval, desiredTime);
            _Config.VerifyAll();
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(10, Time.Minutes, 24, Time.Hours, 1, Time.Hour, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 2, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 3, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 4, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 6, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 8, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 12, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 24, Time.Hours, null)]
        [TestCase(10, Time.Minutes, 24, Time.Hours, 5, Time.Hours, "When Time is Hours, interval must be 1, 2, 3, 4, 6, 8, 12, 24.")]
        public void DetermineErrorMessage_ValidHours(short minInterval, Time minTime, short maxInterval, Time maxTime, short desiredInterval, Time desiredTime, string expectedResult)
        {
            var validHours = new List<short> { 1, 2, 3, 4, 6, 8, 12, 24 };
            _Config.Setup(s => s.Get<List<short>>("ValidHours")).Returns(validHours);
            var minJobConfig = new SubJobConfig(minInterval, minTime);
            var maxJobConfig = new SubJobConfig(maxInterval, maxTime);
            _Config.Setup(s => s.Get<SubJobConfig>("MinJobConfig")).Returns(minJobConfig);
            _Config.Setup(s => s.Get<SubJobConfig>("MaxJobConfig")).Returns(maxJobConfig);
            var result = _TrendHelper.DetermineErrorMessage(desiredInterval, desiredTime);
            _Config.VerifyAll();
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void BuildEmbed()
        {
            var context = new Mock<ICommandContext>();
            const string channelName = "Apex Legends Pog";
            context.Setup(s => s.Channel.Name).Returns(channelName);
            const string guildIconUrl = "https://asdf.com/icon";
            context.Setup(s => s.Guild.IconUrl).Returns(guildIconUrl);
            const string helpText = "Here is some help text to help you out.";
            _Config.Setup(s => s["GetConfigHelpFieldText"]).Returns(helpText);
            var config = new JobConfigContainer();
            var result = _TrendHelper.BuildEmbed(config, context.Object);
            context.VerifyAll();
            _Config.VerifyAll();
            var expectedAuthorName = $"Setup for Channel # {channelName}";
            Assert.That(result.Author.Value.Name, Is.EqualTo(expectedAuthorName));
            Assert.That(result.Author.Value.IconUrl, Is.EqualTo(guildIconUrl));
            Assert.That(result.Fields[0].Inline, Is.True);
            Assert.That(result.Fields[0].Name, Is.EqualTo("How Often?"));
            Assert.That(result.Fields[0].Value, Is.EqualTo("```less\nnever```"));
            Assert.That(result.Fields[1].Inline, Is.True);
            Assert.That(result.Fields[1].Name, Is.EqualTo("Random Gifs?"));
            Assert.That(result.Fields[1].Value, Is.EqualTo("```yaml\nno```"));
            Assert.That(result.Fields[2].Inline, Is.True);
            Assert.That(result.Fields[2].Name, Is.EqualTo("Trend When?"));
            Assert.That(result.Fields[2].Value, Is.EqualTo("```fix\nall the time```"));
            Assert.That(result.Fields[3].Inline, Is.False);
            Assert.That(result.Fields[3].Name, Is.EqualTo("Need Help?"));
            Assert.That(result.Fields[3].Value, Is.EqualTo(helpText));
        }
    }
}
