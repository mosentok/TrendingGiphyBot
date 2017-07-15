using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Attributes
{
    class JobConfigSetSummaryAttribute : SummaryAttribute
    {
        public JobConfigSetSummaryAttribute(string text, int minimumSeconds, int maximumSeconds)
            : base($"{text}\n"
                  + $"{ModuleBaseHelper.InvalidConfigRange(minimumSeconds, maximumSeconds)}\n"
                  + $"{ModuleBaseHelper.InvalidConfig(Time.Hour, ModuleBaseHelper.ValidHoursString)}\n"
                  + $"{ModuleBaseHelper.InvalidConfig(Time.Minutes, ModuleBaseHelper.ValidMinutesSecondsString)}\n"
                  + $"{ModuleBaseHelper.InvalidConfig(Time.Seconds, ModuleBaseHelper.ValidMinutesSecondsString)}\n"
                  + "*Parameters*:") { }
    }
}
