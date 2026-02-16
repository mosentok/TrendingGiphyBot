using Discord;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsButtonBuilder(
    IChannelSettingsDisplay _display
) : IChannelSettingsButtonBuilder
{
    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGifRetentionButtons(ChannelSettingsDto channelSettings)
    {
        var displayText = _display.DetermineGifRetentionDisplay(channelSettings);
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Retention Period ({displayText})");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifRetentionOpenButton)
            .WithLabel(setButtonLabel)
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetRetentionPeriodButton)
            .WithLabel("Reset Retention Period")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifSourcesButtons(ChannelSettingsDto channelSettings)
    {
        var displayText = _display.DetermineGifSourceDisplay(channelSettings);
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Gif Sources ({displayText})");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourcesOpenButton)
            .WithLabel(setButtonLabel)
            .WithStyle(ButtonStyle.Primary);

        var clearButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearGifSourcesButton)
            .WithLabel("Clear Gif Sources")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, clearButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder clearButton) BuildGifKeywordButtons(ChannelSettingsDto channelSettings)
    {
        var keywordDisplay = channelSettings.GifKeyword ?? "<none>";
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Random Gif Keywords ({keywordDisplay})");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithKeywordButton)
            .WithLabel(setButtonLabel)
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
        var displayText = _display.DeterminePostingHoursDisplay(channelSettings);
        var displaySuffix = string.IsNullOrEmpty(displayText) ? string.Empty : $" ({displayText})";
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Posting Hours{displaySuffix}");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingPostingHoursButton)
            .WithLabel(setButtonLabel)
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
        var displayText = _display.DeterminePostingBehaviorDisplay(channelSettings);
        var displaySuffix = string.IsNullOrEmpty(displayText) ? string.Empty : $" ({displayText})";
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Posting Behavior{displaySuffix}");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorOpenButton)
            .WithLabel(setButtonLabel)
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetPostingBehaviorButton)
            .WithLabel("Reset Posting Behavior")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildHowOftenButtons(ChannelSettingsDto channelSettings)
    {
        var displayText = _display.DetermineHowOftenDisplay(channelSettings);
        var displaySuffix = string.IsNullOrEmpty(displayText) ? string.Empty : $" ({displayText})";
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set How Often to Post Gifs{displaySuffix}");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.HowOftenOpenButton)
            .WithLabel(setButtonLabel)
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetHowOftenButton)
            .WithLabel("Reset How Often")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }

    public (ButtonBuilder setButton, ButtonBuilder resetButton) BuildGiphyRatingButtons(ChannelSettingsDto channelSettings)
    {
        var displayText = _display.DetermineGiphyRatingDisplay(channelSettings);
        var setButtonLabel = _display.TrimLabelTo80Chars($"Set Giphy Rating ({displayText})");

        var setButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GiphyRatingOpenButton)
            .WithLabel(setButtonLabel)
            .WithStyle(ButtonStyle.Primary);

        var resetButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetGiphyRatingButton)
            .WithLabel("Reset Giphy Rating")
            .WithStyle(ButtonStyle.Danger);

        return (setButton, resetButton);
    }
}
