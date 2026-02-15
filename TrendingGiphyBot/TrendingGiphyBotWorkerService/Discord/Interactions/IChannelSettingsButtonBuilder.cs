using Discord;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public interface IChannelSettingsButtonBuilder
{
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGifRetentionButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifSourcesButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifKeywordButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildPostingHoursButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildPostingBehaviorButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildHowOftenButtons(ChannelSettingsModel channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGiphyRatingButtons(ChannelSettingsModel channelSettings);
}
