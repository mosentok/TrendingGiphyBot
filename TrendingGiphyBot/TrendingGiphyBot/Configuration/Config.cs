using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Configuration
{
    public class Config
    {
        [JsonRequired]
        public string DiscordToken { get; set; }
        [JsonRequired]
        public SubJobConfig MinJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig MaxJobConfig { get; set; }
        [JsonRequired]
        public List<int> ValidMinutes { get; set; }
        [JsonRequired]
        public List<int> ValidHours { get; set; }
        [JsonRequired]
        public List<LogSeverity> LogSeverities { get; set; }
        [JsonRequired]
        public string DefaultPrefix { get; set; }
        [JsonRequired]
        public string PlayingGame { get; set; }
        [JsonRequired]
        public ushort RandomSearchStringMaxLength { get; set; }
        [JsonRequired]
        public ulong OwnerId { get; set; }
        [JsonRequired]
        public List<string> HttpExceptionsToWarn { get; set; }
        [JsonRequired]
        public string ExamplesHelpFieldText { get; set; }
        [JsonRequired]
        public string ExamplesText { get; set; }
        [JsonRequired]
        public string GetConfigHelpFieldText { get; set; }
        [JsonRequired]
        public string InvalidQuietHoursRangeMessage { get; set; }
        [JsonRequired]
        public string InvalidQuietHoursInputFormat { get; set; }
        [JsonRequired]
        public string QuietHoursMustBeDifferentMessage { get; set; }
        internal JobConfigState DetermineJobConfigState(int interval, Time time)
        {
            var minTimeSpan = AsTimeSpan(MinJobConfig);
            var maxTimeSpan = AsTimeSpan(MaxJobConfig);
            var desiredTimeSpan = AsTimeSpan(interval, time);
            if (desiredTimeSpan >= minTimeSpan)
            {
                if (desiredTimeSpan <= maxTimeSpan)
                    return DetermineTimeState(interval, time);
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        JobConfigState DetermineTimeState(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    if (ValidHours.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidHours;
                case Time.Minute:
                case Time.Minutes:
                    if (ValidMinutes.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidMinutes;
                default:
                    return JobConfigState.InvalidTime;
            }
        }
        static TimeSpan AsTimeSpan(SubJobConfig config) => AsTimeSpan(config.Interval, config.Time);
        static TimeSpan AsTimeSpan(int interval, Time time)
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
    }
}
