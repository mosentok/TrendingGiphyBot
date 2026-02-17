using Discord;
using TrendingGiphyBotWorkerService.Discord.Interactions;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsMessageComponentFactory
{
    MessageComponent BuildChannelSettingsMessageComponent(
        ChannelSettingsDto channelSettings,
        string channelName);
    MessageComponent BuildHowOftenToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null);
    MessageComponent BuildPostingBehaviorToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null);
    MessageComponent BuildGifSourcesToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null);
    MessageComponent BuildGifRetentionToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null);
    MessageComponent BuildGiphyRatingToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null);
    MessageComponent BuildConfirmationModal(string message, string channelName);
}
