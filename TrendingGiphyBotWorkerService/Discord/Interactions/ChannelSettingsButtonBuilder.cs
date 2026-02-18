using Discord;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsButtonBuilder(IOptionsMonitor<AppConfig> _appConfig) : IChannelSettingsButtonBuilder
{
    public (ButtonBuilder setButton, ButtonBuilder? resetButton) BuildGifRetentionButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifRetentionOpenButton)
            .WithLabel("Set Retention Period")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = _appConfig.CurrentValue.ButtonVisibility.EnableResetGifRetentionButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ResetRetentionPeriodButton)
                .WithLabel("Reset Retention Period")
                .WithStyle(ButtonStyle.Danger)
            : null;

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? clearButton) BuildGifSourcesButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourcesOpenButton)
            .WithLabel("Set Gif Sources")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = _appConfig.CurrentValue.ButtonVisibility.EnableClearGifSourcesButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ClearGifSourcesButton)
                .WithLabel("Clear Gif Sources")
                .WithStyle(ButtonStyle.Danger)
                .WithDisabled(channelSettings.GifSource == GifSourceKind.None)
            : null;

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? clearButton) BuildGifKeywordButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithKeywordButton)
            .WithLabel("Set Random Gif Keywords")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = _appConfig.CurrentValue.ButtonVisibility.EnableClearKeywordButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ClearKeywordButton)
                .WithLabel("Clear Random Gif Keywords")
                .WithStyle(ButtonStyle.Danger)
                .WithDisabled(channelSettings.GifKeyword is null)
            : null;

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? clearButton) BuildPostingHoursButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingPostingHoursButton)
            .WithLabel("Set Posting Hours")
            .WithStyle(ButtonStyle.Primary);

        var clearButton = _appConfig.CurrentValue.ButtonVisibility.EnableClearPostingHoursButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ClearPostingHoursButton)
                .WithLabel("Clear Posting Hours")
                .WithStyle(ButtonStyle.Danger)
                .WithDisabled(channelSettings.PostingHours?.From is null || channelSettings.PostingHours.To is null)
            : null;

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? resetButton) BuildPostingBehaviorButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorOpenButton)
            .WithLabel("Set Posting Behavior")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = _appConfig.CurrentValue.ButtonVisibility.EnableResetPostingBehaviorButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ResetPostingBehaviorButton)
                .WithLabel("Reset Posting Behavior")
                .WithStyle(ButtonStyle.Danger)
            : null;

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? resetButton) BuildHowOftenButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.HowOftenOpenButton)
            .WithLabel("Set How Often to Post Gifs")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = _appConfig.CurrentValue.ButtonVisibility.EnableResetHowOftenButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ResetHowOftenButton)
                .WithLabel("Reset How Often")
                .WithStyle(ButtonStyle.Danger)
            : null;

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder? resetButton) BuildGiphyRatingButtons(ChannelSettingsDto channelSettings)
    {
        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GiphyRatingOpenButton)
            .WithLabel("Set Giphy Rating")
            .WithStyle(ButtonStyle.Primary);

        var resetButton = _appConfig.CurrentValue.ButtonVisibility.EnableResetGiphyRatingButton
            ? new ButtonBuilder()
                .WithCustomId(InteractionId.ResetGiphyRatingButton)
                .WithLabel("Reset Giphy Rating")
                .WithStyle(ButtonStyle.Danger)
            : null;

        return (setButton, resetButton);
    }
}
