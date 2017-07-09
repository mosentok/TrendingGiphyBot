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
                var configgedMinutes = DetermineJobIntervalSeconds();
                return configgedMinutes >= MinMinutes;
            }
        }
        double DetermineJobIntervalSeconds()
        {
            switch (Time)
            {
                case Time.Hours:
                    return (int)TimeSpan.FromHours(Interval).TotalMinutes;
                case Time.Minutes:
                    return (int)TimeSpan.FromMinutes(Interval).TotalMinutes;
                case Time.Seconds:
                    return (int)TimeSpan.FromSeconds(Interval).TotalMinutes;
                default:
                    throw new InvalidTimeException(Time);
            }
        }
    }
}
