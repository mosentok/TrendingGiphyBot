using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;

namespace TrendingGiphyBotCore.Helpers
{
    public class TrendHelper : ITrendHelper
    {
        public event Action<decimal, string> PrefixUpdated;
        static readonly string[] _FluentWords = new[] {"gifs of", "gif of", "gifs", "gif" };
        readonly List<short> _ValidMinutes;
        readonly List<short> _ValidHours;
        readonly SubJobConfig _MinJobConfig;
        readonly SubJobConfig _MaxJobConfig;
        public TrendHelper(IConfiguration config)
        {
            _ValidMinutes = config.Get<List<short>>("ValidMinutes");
            _ValidHours = config.Get<List<short>>("ValidHours");
            _MinJobConfig = config.Get<SubJobConfig>("MinJobConfig");
            _MaxJobConfig = config.Get<SubJobConfig>("MaxJobConfig");
        }
        public string CleanRandomSearchString(string randomSearchString)
        {
            if (!string.IsNullOrEmpty(randomSearchString))
            {
                var match = _FluentWords.FirstOrDefault(s => randomSearchString.StartsWith(s, StringComparison.CurrentCultureIgnoreCase));
                if (match != null)
                    return randomSearchString.Substring(match.Length).TrimStart();
                return randomSearchString;
            }
            return null;
        }
        public bool IsValidRandomSearchString(string cleanedRandomSearchString, int randomSearchStringMaxLength) => string.IsNullOrWhiteSpace(cleanedRandomSearchString) || cleanedRandomSearchString.Length <= randomSearchStringMaxLength;
        public bool IsInRange(string quietHourString, out short quietHour)
        {
            var success = short.TryParse(quietHourString, out quietHour);
            return success && 0 <= quietHour && quietHour <= 23;
        }
        public string InvalidHoursConfigMessage(Time time) => InvalidConfigMessage(time, _ValidHours);
        public string InvalidMinutesConfigMessage(Time time) => InvalidConfigMessage(time, _ValidMinutes);
        static string InvalidConfigMessage(Time time, List<short> validValues) => $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        public string InvalidConfigRangeMessage() => $"Interval must be between {_MinJobConfig.Interval} {_MinJobConfig.Time} and {_MaxJobConfig.Interval} {_MaxJobConfig.Time}.";
        public bool ShouldTurnCommandOff(string word) => "Off".Equals(word, StringComparison.CurrentCultureIgnoreCase);
        public JobConfigState DetermineJobConfigState(short interval, Time time)
        {
            var minTimeSpan = AsTimeSpan(_MinJobConfig);
            var maxTimeSpan = AsTimeSpan(_MaxJobConfig);
            var desiredTimeSpan = AsTimeSpan(interval, time);
            if (desiredTimeSpan >= minTimeSpan)
            {
                if (desiredTimeSpan <= maxTimeSpan)
                    return DetermineTimeState(interval, time);
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        JobConfigState DetermineTimeState(short interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    if (_ValidHours.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidHours;
                case Time.Minute:
                case Time.Minutes:
                    if (_ValidMinutes.Contains(interval))
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
        public void OnPrefixUpdated(decimal channelId, string prefix)
        {
            PrefixUpdated?.Invoke(channelId, prefix);
        }
    }
}
