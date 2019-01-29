using System.Collections.Generic;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;

namespace TrendingGiphyBotCore.Helpers
{
    public interface ITrendHelper
    {
        string CleanRandomSearchString(string randomSearchString);
        bool IsValidRandomSearchString(string cleanedRandomSearchString, int randomSearchStringMaxLength);
        bool IsInRange(string quietHourString, out short quietHour);
        string InvalidConfigMessage(Time time, List<int> validValues);
        string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig);
        bool ShouldTurnCommandOff(string word);
    }
}