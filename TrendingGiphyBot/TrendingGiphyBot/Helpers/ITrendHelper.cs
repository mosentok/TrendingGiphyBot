using System.Collections.Generic;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Helpers
{
    public interface ITrendHelper
    {
        string CleanRandomSearchString(string randomSearchString);
        bool IsValidRandomSearchString(string cleanedRandomSearchString, int randomSearchStringMaxLength);
        string InvalidConfigMessage(Time time, List<int> validValues);
        string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig);
        bool IsWordThatStopsCommands(List<string> wordsThatStopCommands, string word);
    }
}