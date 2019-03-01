using TrendingGiphyBotCore.Enums;

namespace TrendingGiphyBotCore.Helpers
{
    public interface ITrendHelper
    {
        bool IsInRange(string quietHourString, out short quietHour);
        string InvalidConfigRangeMessage();
        bool ShouldTurnCommandOff(string word);
        JobConfigState DetermineJobConfigState(short interval, Time time);
        string InvalidHoursConfigMessage(Time time);
        string InvalidMinutesConfigMessage(Time time);
        void OnPrefixUpdated(decimal channelId, string prefix);
    }
}