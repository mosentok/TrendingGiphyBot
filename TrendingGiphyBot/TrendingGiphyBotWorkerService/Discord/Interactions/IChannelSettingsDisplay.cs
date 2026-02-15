using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public interface IChannelSettingsDisplay
{
    string DetermineGifRetentionDisplay(ChannelSettingsModel channelSettings);
    string DetermineGifSourceDisplay(ChannelSettingsModel channelSettings);
    string DetermineGiphyRatingDisplay(ChannelSettingsModel channelSettings);
    string DetermineHowOftenDisplay(ChannelSettingsModel channelSettings);
    string DeterminePostingBehaviorDisplay(ChannelSettingsModel channelSettings);
    string DeterminePostingHoursDisplay(ChannelSettingsModel channelSettings);
    string TrimLabelTo80Chars(string label);
}
