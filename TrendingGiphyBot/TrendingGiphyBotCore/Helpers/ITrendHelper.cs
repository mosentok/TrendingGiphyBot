using Discord;
using Discord.Commands;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotModel;

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
        Embed BuildEmbed(JobConfigContainer config, ICommandContext context);
        void OnPrefixUpdated(decimal channelId, string prefix);
    }
}