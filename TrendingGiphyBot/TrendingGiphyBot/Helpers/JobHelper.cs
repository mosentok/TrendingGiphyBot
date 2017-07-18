using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Helpers
{
    static class JobHelper
    {
        internal static JobConfigState DetermineJobConfigState(int interval, Time time, Config config)
        {
            var minSeconds = DetermineConfiggedSeconds(config.MinJobConfig.Interval, config.MinJobConfig.Time);
            var maxSeconds = DetermineConfiggedSeconds(config.MaxJobConfig.Interval, config.MaxJobConfig.Time);
            var configgedSeconds = DetermineConfiggedSeconds(interval, time);
            if (configgedSeconds >= minSeconds)
            {
                if (configgedSeconds <= maxSeconds)
                    switch (time)
                    {
                        case Time.Hour:
                        case Time.Hours:
                            if (config.ValidHours.Any())
                            {
                                if (config.ValidHours.Contains(interval))
                                    return JobConfigState.Valid;
                                return JobConfigState.InvalidHours;
                            }
                            return JobConfigState.InvalidTime;
                        case Time.Minute:
                        case Time.Minutes:
                            if (config.ValidMinutes.Any())
                                return IsValid(interval, JobConfigState.InvalidMinutes, config.ValidMinutes);
                            return JobConfigState.InvalidMinutes;
                        case Time.Second:
                        case Time.Seconds:
                            if (config.ValidSeconds.Any())
                                return IsValid(interval, JobConfigState.InvalidSeconds, config.ValidSeconds);
                            return JobConfigState.InvalidTime;
                        default:
                            return JobConfigState.InvalidTime;
                    }
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        static JobConfigState IsValid(int interval, JobConfigState invalidState, List<int> validMinutes)
        {
            var isValidMinuteSecond = validMinutes.Contains(interval);
            if (isValidMinuteSecond)
                return JobConfigState.Valid;
            return invalidState;
        }
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
                    throw new InvalidTimeException(time);
            }
        }
    }
}
