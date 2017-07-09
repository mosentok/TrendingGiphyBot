using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TrendingGiphyBot
{
    public class JobConfig
    {
        internal int MinMinutes => 10;
        internal string MinMinutesMessage => $"{nameof(Interval)} and {nameof(Time)} must combine to at least {MinMinutes} minutes.";
        public int Interval { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Time Time { get; set; }
        internal bool IsValid
        {
            get
            {
                var minSeconds = TimeSpan.FromMinutes(MinMinutes).TotalSeconds;
                var configgedSeconds = DetermineJobIntervalSeconds();
                return configgedSeconds >= minSeconds;
            }
        }
        internal double DetermineJobIntervalSeconds()
        {
            switch (Time)
            {
                case Time.Hours:
                    return (int)TimeSpan.FromHours(Interval).TotalSeconds;
                case Time.Minutes:
                    return (int)TimeSpan.FromMinutes(Interval).TotalSeconds;
                case Time.Seconds:
                    return (int)TimeSpan.FromSeconds(Interval).TotalSeconds;
                default:
                    throw new InvalidOperationException($"{Time} is an invalid {nameof(Time)}.");
            }
        }
    }
}
