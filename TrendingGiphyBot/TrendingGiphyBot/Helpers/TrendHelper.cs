using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Helpers
{
    public class TrendHelper : ITrendHelper
    {
        public string CleanRandomSearchString(string randomSearchString)
        {
            if (!string.IsNullOrEmpty(randomSearchString))
            {
                if (randomSearchString.StartsWith("gifs of", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(7).TrimStart();
                if (randomSearchString.StartsWith("gif of", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(6).TrimStart();
                if (randomSearchString.StartsWith("gifs", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(4).TrimStart();
                if (randomSearchString.StartsWith("gif", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(3).TrimStart();
                return randomSearchString;
            }
            return null;
        }
        public bool IsValidRandomSearchString(string cleanedRandomSearchString, int randomSearchStringMaxLength) => string.IsNullOrWhiteSpace(cleanedRandomSearchString) || cleanedRandomSearchString.Length <= randomSearchStringMaxLength;
        public string InvalidConfigMessage(Time time, List<int> validValues) => $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        public string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) => $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
        public bool IsWordThatStopsCommands(List<string> wordsThatStopCommands, string word) => wordsThatStopCommands.Any(s => s.Equals(word, StringComparison.CurrentCultureIgnoreCase));
    }
}
