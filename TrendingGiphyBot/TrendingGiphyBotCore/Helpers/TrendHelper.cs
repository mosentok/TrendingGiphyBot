﻿using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;

namespace TrendingGiphyBotCore.Helpers
{
    public class TrendHelper : ITrendHelper
    {
        static readonly string[] _FluentWords = new[] {"gifs of", "gif of", "gifs", "gif" };
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
        public string InvalidConfigMessage(Time time, List<int> validValues) => $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        public string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) => $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
        public bool ShouldTurnCommandOff(string word) => "Off".Equals(word, StringComparison.CurrentCultureIgnoreCase);
    }
}
