using Discord;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsMessageComponentFactory
{
    MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName);
    MessageComponent BuildHowOftenToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null);
    MessageComponent BuildPostingBehaviorToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null);
    MessageComponent BuildGifSourcesToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null);
    MessageComponent BuildGifRetentionToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null);
    MessageComponent BuildGiphyRatingToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null);
    MessageComponent BuildConfirmationModal(string message);
}
