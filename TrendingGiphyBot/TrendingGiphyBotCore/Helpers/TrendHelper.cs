using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;
using TrendingGiphyBotCore.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Helpers
{
    public class TrendHelper : ITrendHelper
    {
        public event Action<decimal, string> PrefixUpdated;
        readonly IConfigurationWrapper _Config;
        public TrendHelper(IConfigurationWrapper config)
        {
            _Config = config;
        }
        public bool IsInRange(string quietHourString, out short quietHour)
        {
            var success = short.TryParse(quietHourString, out quietHour);
            return success && 0 <= quietHour && quietHour <= 23;
        }
        public string InvalidHoursConfigMessage(Time time)
        {
            var validHours = _Config.Get<List<short>>("ValidHours");
            return InvalidConfigMessage(time, validHours);
        }
        public string InvalidMinutesConfigMessage(Time time)
        {
            var validMinutes = _Config.Get<List<short>>("ValidMinutes");
            return InvalidConfigMessage(time, validMinutes);
        }
        static string InvalidConfigMessage(Time time, List<short> validValues) => $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        public string InvalidConfigRangeMessage()
        {
            var minJobConfig = _Config.Get<SubJobConfig>("MinJobConfig");
            var maxJobConfig = _Config.Get<SubJobConfig>("MaxJobConfig");
            return $"Interval must be between {minJobConfig.Interval} {minJobConfig.Time} and {maxJobConfig.Interval} {maxJobConfig.Time}.";
        }
        public bool ShouldTurnCommandOff(string word) => "Off".Equals(word, StringComparison.CurrentCultureIgnoreCase);
        public JobConfigState DetermineJobConfigState(short interval, Time time)
        {
            var minJobConfig = _Config.Get<SubJobConfig>("MinJobConfig");
            var maxJobConfig = _Config.Get<SubJobConfig>("MaxJobConfig");
            var minTimeSpan = AsTimeSpan(minJobConfig);
            var maxTimeSpan = AsTimeSpan(maxJobConfig);
            var desiredTimeSpan = AsTimeSpan(interval, time);
            if (desiredTimeSpan < minTimeSpan)
                return JobConfigState.IntervalTooSmall;
            if (desiredTimeSpan > maxTimeSpan)
                return JobConfigState.IntervallTooBig;
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    var validHours = _Config.Get<List<short>>("ValidHours");
                    if (validHours.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidHours;
                case Time.Minute:
                case Time.Minutes:
                    var validMinutes = _Config.Get<List<short>>("ValidMinutes");
                    if (validMinutes.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidMinutes;
                default:
                    return JobConfigState.InvalidTime;
            }
        }
        static TimeSpan AsTimeSpan(SubJobConfig config) => AsTimeSpan(config.Interval, config.Time);
        static TimeSpan AsTimeSpan(short interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval);
                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval);
                default:
                    throw new UnexpectedTimeException(time);
            }
        }
        public Embed BuildEmbed(JobConfigContainer config, ICommandContext context)
        {
            var author = new EmbedAuthorBuilder()
                .WithName($"Setup for Channel # {context.Channel.Name}")
                .WithIconUrl(context.Guild.IconUrl);
            var helpFieldText = _Config["GetConfigHelpFieldText"];
            var helpField = new EmbedFieldBuilder()
                .WithName("Need Help?")
                .WithValue(helpFieldText);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithHowOften(config)
                .WithRandomConfigFields(config)
                .WithQuietHourFields(config)
                .AddField(helpField);
            return embedBuilder.Build();
        }
        public void OnPrefixUpdated(decimal channelId, string prefix)
        {
            PrefixUpdated?.Invoke(channelId, prefix);
        }
    }
}
