using Discord;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsButtonBuilder : IChannelSettingsButtonBuilder
{
    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGifRetentionButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifRetentionOpenButton)
            .WithLabel("Set Retention Period")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetRetentionPeriodButton)
            .WithLabel("Reset Retention Period")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifSourcesButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourcesOpenButton)
            .WithLabel("Set Gif Sources")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearGifSourcesButton)
            .WithLabel("Clear Gif Sources")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifKeywordButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithKeywordButton)
            .WithLabel("Set Random Gif Keywords")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearKeywordButton)
            .WithLabel("Clear Random Gif Keywords")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.GifKeyword is null);

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder clearButton) BuildPostingHoursButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingPostingHoursButton)
            .WithLabel("Set Posting Hours")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearPostingHoursButton)
            .WithLabel("Clear Posting Hours")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null);

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildPostingBehaviorButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorOpenButton)
            .WithLabel("Set Posting Behavior")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetPostingBehaviorButton)
            .WithLabel("Reset Posting Behavior")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildHowOftenButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.HowOftenOpenButton)
            .WithLabel("Set How Often to Post Gifs")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetHowOftenButton)
            .WithLabel("Reset How Often")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGiphyRatingButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GiphyRatingOpenButton)
            .WithLabel("Set Giphy Rating")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetGiphyRatingButton)
            .WithLabel("Reset Giphy Rating")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }
}
