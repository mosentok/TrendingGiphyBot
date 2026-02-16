using Discord;
using TrendingGiphyBotWorkerService.Discord.Interactions;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsMessageComponentFactory
{
    MessageComponent BuildChannelSettingsMessageComponent(
        ChannelSettingsDto channelSettings,
        string channelName);
    MessageComponent BuildHowOftenToggleModal(ChannelSettingsDto channelSettings, string? selectedValue = null);
    MessageComponent BuildPostingBehaviorToggleModal(ChannelSettingsDto channelSettings, string? selectedValue = null);
    MessageComponent BuildGifSourcesToggleModal(ChannelSettingsDto channelSettings, string? selectedValue = null);
    MessageComponent BuildGifRetentionToggleModal(ChannelSettingsDto channelSettings, string? selectedValue = null);
    MessageComponent BuildGiphyRatingToggleModal(ChannelSettingsDto channelSettings, string? selectedValue = null);
    MessageComponent BuildConfirmationModal(string message);
}
