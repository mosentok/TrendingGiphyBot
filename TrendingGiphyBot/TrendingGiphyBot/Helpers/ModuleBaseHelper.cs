using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Helpers
{
    static class ModuleBaseHelper
    {
        internal static string InvalidConfigMessage(Time time, string validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {validValues}.";
        internal static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
    }
}
