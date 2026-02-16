using Discord;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public interface IChannelSettingsButtonBuilder
{
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGifRetentionButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifSourcesButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifKeywordButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder clearButton) BuildPostingHoursButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildPostingBehaviorButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildHowOftenButtons(ChannelSettingsDto channelSettings);
    (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGiphyRatingButtons(ChannelSettingsDto channelSettings);
}
