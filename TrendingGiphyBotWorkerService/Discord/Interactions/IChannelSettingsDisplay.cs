namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public interface IChannelSettingsDisplay
{
    string DetermineGifRetentionDisplay(ChannelSettingsDto channelSettings);
    string DetermineGifSourceDisplay(ChannelSettingsDto channelSettings);
    string DetermineGiphyRatingDisplay(ChannelSettingsDto channelSettings);
    string DetermineHowOftenDisplay(ChannelSettingsDto channelSettings);
    string DeterminePostingBehaviorDisplay(ChannelSettingsDto channelSettings);
    string DeterminePostingHoursDisplay(ChannelSettingsDto channelSettings);
    string TrimLabelTo80Chars(string label);
}
