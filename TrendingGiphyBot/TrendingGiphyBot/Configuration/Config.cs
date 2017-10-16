using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Configuration
{
    public class Config
    {
        [JsonRequired]
        public string DiscordToken { get; set; }
        [JsonRequired]
        public string GiphyToken { get; set; }
        [JsonRequired]
        public string ConnectionString { get; set; }
        [JsonRequired]
        public SubJobConfig RefreshImageJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig MinJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig MaxJobConfig { get; set; }
        [JsonRequired]
        public List<int> ValidSeconds { get; set; }
        [JsonRequired]
        public List<int> ValidMinutes { get; set; }
        [JsonRequired]
        public List<int> ValidHours { get; set; }
        [JsonRequired]
        public List<LogSeverity> LogSeverities { get; set; }
        [JsonRequired]
        public string DefaultPrefix { get; set; }
        [JsonRequired]
        public string GitHubUrl { get; set; }
        [JsonRequired]
        public string PlayingGame { get; set; }
        [JsonRequired]
        public short HourOffset { get; set; }
        [JsonRequired]
        public ushort RandomSearchStringMaxLength { get; set; }
        [JsonRequired]
        public ulong OwnerId { get; set; }
        [JsonRequired]
        public SubJobConfig DefaultJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig DeleteOldUrlHistoriesJobConfig { get; set; }
        [JsonRequired]
        public ushort UrlHistoriesMaxDaysOld { get; set; }
        [JsonRequired]
        public SubJobConfig DeleteOldUrlCachesJobConfig { get; set; }
        [JsonRequired]
        public ushort UrlCachesMaxDaysOld { get; set; }
        [JsonRequired]
        public EmbedConfig WelcomeMessageDefault { get; set; }
        [JsonRequired]
        public EmbedConfig WelcomeMessageOwner { get; set; }
        [JsonRequired]
        public EmbedConfig HelpMessage { get; set; }
        public List<StatPost> StatPosts { get; set; }
        [JsonRequired]
        public ushort IntervalOffsetSeconds { get; set; }
        [JsonRequired]
        public List<string> UrlsToIgnore { get; set; }
        [JsonRequired]
        public List<string> HttpExceptionsToWarn { get; set; }
        internal JobConfigState DetermineJobConfigState(int interval, Time time)
        {
            var minSeconds = DetermineConfiggedSeconds(MinJobConfig);
            var maxSeconds = DetermineConfiggedSeconds(MaxJobConfig);
            var configgedSeconds = DetermineConfiggedSeconds(interval, time);
            if (configgedSeconds >= minSeconds)
            {
                if (configgedSeconds <= maxSeconds)
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
                    if (ValidHours.Any())
                    {
                        if (ValidHours.Contains(interval))
                            return JobConfigState.Valid;
                        return JobConfigState.InvalidHours;
                    }
                    return JobConfigState.InvalidTime;
                case Time.Minute:
                case Time.Minutes:
                    if (ValidMinutes.Any())
                        return IsValid(interval, JobConfigState.InvalidMinutes, ValidMinutes);
                    return JobConfigState.InvalidMinutes;
                case Time.Second:
                case Time.Seconds:
                    if (ValidSeconds.Any())
                        return IsValid(interval, JobConfigState.InvalidSeconds, ValidSeconds);
                    return JobConfigState.InvalidTime;
                default:
                    return JobConfigState.InvalidTime;
            }
        }
        static JobConfigState IsValid(int interval, JobConfigState invalidState, List<int> validMinutes)
        {
            var isValidMinuteSecond = validMinutes.Contains(interval);
            if (isValidMinuteSecond)
                return JobConfigState.Valid;
            return invalidState;
        }
        static double DetermineConfiggedSeconds(SubJobConfig config) => DetermineConfiggedSeconds(config.Interval, config.Time);
        static double DetermineConfiggedSeconds(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval).TotalSeconds;
                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).TotalSeconds;
                case Time.Second:
                case Time.Seconds:
                    return TimeSpan.FromSeconds(interval).TotalSeconds;
                default:
                    throw new UnexpectedTimeException(time);
            }
        }
    }
}
