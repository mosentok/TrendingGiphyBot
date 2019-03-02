using Discord;
using Discord.Commands;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Helpers
{
    public interface ITrendHelper
    {
        bool IsInRange(string quietHourString, out short quietHour);
        bool ShouldTurnCommandOff(string word);
        string DetermineErrorMessage(short interval, Time time);
        Embed BuildEmbed(JobConfigContainer config, ICommandContext context);
        void OnPrefixUpdated(decimal channelId, string prefix);
    }
}