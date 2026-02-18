using Discord;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsMessageComponentFactory(
    IOptionsMonitor<AppConfig> _appConfig,
    IChannelSettingsButtonBuilder _buttonBuilder,
    ICurrentChannelSettingsDisplayBuilder _currentSettingsDisplayBuilder
) : IChannelSettingsMessageComponentFactory
{
    public MessageComponent BuildChannelSettingsMessageComponent(
        ChannelSettingsDto channelSettings,
        string channelName)
    {
        var (howOftenButton, resetHowOftenButton) = _buttonBuilder.BuildHowOftenButtons(channelSettings);
        var (postingBehaviorButton, resetPostingBehaviorButton) = _buttonBuilder.BuildPostingBehaviorButtons(channelSettings);
        var (gifSourcesButton, clearGifSourcesButton) = _buttonBuilder.BuildGifSourcesButtons(channelSettings);
        var (gifRetentionButton, resetGifRetentionButton) = _buttonBuilder.BuildGifRetentionButtons(channelSettings);
        var (giphyRatingButton, resetGiphyRatingButton) = _buttonBuilder.BuildGiphyRatingButtons(channelSettings);
        var (gifKeywordButton, clearGifKeywordButton) = _buttonBuilder.BuildGifKeywordButtons(channelSettings);
        var (setPostingHoursButton, clearPostingHoursButton) = _buttonBuilder.BuildPostingHoursButtons(channelSettings);

        var mainSettingsDisplay = _currentSettingsDisplayBuilder.BuildMainSettingsDisplay(channelSettings);
        var optionalSettingsDisplay = _currentSettingsDisplayBuilder.BuildOptionalSettingsDisplay(channelSettings);

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithSeparator()
            .WithTextDisplay($"## Main Settings for {channelName}")
            .WithTextDisplay(mainSettingsDisplay)
            .WithActionRow([howOftenButton, resetHowOftenButton])
            .WithActionRow([postingBehaviorButton, resetPostingBehaviorButton])
            .WithSeparator()
            .WithTextDisplay($"## Optional Settings for {channelName}")
            .WithTextDisplay(optionalSettingsDisplay)
            .WithActionRow([gifSourcesButton, clearGifSourcesButton])
            .WithActionRow([gifRetentionButton, resetGifRetentionButton])
            .WithActionRow([giphyRatingButton, resetGiphyRatingButton])
            .WithActionRow([gifKeywordButton, clearGifKeywordButton])
            .WithActionRow([setPostingHoursButton, clearPostingHoursButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls);

        return componentBuilder.Build();
    }

    public MessageComponent BuildConfirmationModal(string message, string channelName)
    {
        var confirmButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ConfirmClearButton)
            .WithLabel("Confirm")
            .WithStyle(ButtonStyle.Danger);

        var cancelButton = new ButtonBuilder()
            .WithCustomId(InteractionId.CancelClearButton)
            .WithLabel("Cancel")
            .WithStyle(ButtonStyle.Secondary);

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithSeparator()
            .WithTextDisplay(message)
            .WithActionRow([cancelButton, confirmButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls);

        return componentBuilder.Build();
    }

    public MessageComponent BuildGifRetentionToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null)
    {
        var defaultRetentionDays = _appConfig.CurrentValue.GifRetention.DefaultDays;
        var effectiveRetentionDays = channelSettings.RetentionDays ?? defaultRetentionDays;

        var buttons = new List<ButtonBuilder>();

        foreach (var days in _appConfig.CurrentValue.GifRetention.Days)
        {
            var label = $"{days} days";

            buttons.Add(
                new ButtonBuilder()
                    .WithCustomId($"{InteractionId.GifRetentionButtonPrefix}:{days}")
                    .WithLabel(label)
                    .WithStyle(ButtonStyle.Primary)
            );
        }

        var selected = selectedValue ?? effectiveRetentionDays.ToString();
        var selectedButtonId = $"{InteractionId.GifRetentionButtonPrefix}:{selected}";
        var buttonsArray = buttons.ToArray();

        foreach (var button in buttonsArray)
        {
            var customId = button.CustomId;

            if (customId == selectedButtonId)
                button.WithStyle(ButtonStyle.Success);
        }

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithTextDisplay("## Set Retention Period");

        var chunkedButtons = buttonsArray.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        return componentBuilder
            .WithSeparator()
            .WithActionRow([backButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls)
            .Build();
    }

    public MessageComponent BuildGifSourcesToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null)
    {
        var allGifSources = Enum.GetValues<GifSourceKind>();
        var channelGifSource = channelSettings.GifSource ?? allGifSources.Aggregate((left, right) => left | right);

        var giphyButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourceGiphyButton)
            .WithLabel("Giphy")
            .WithStyle(ButtonStyle.Primary);

        var klipyButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourceKlipyButton)
            .WithLabel("Klipy")
            .WithStyle(ButtonStyle.Primary);

        var buttons = new[] { giphyButton, klipyButton };

        if (channelGifSource.HasFlag(GifSourceKind.Giphy))
            giphyButton.WithStyle(ButtonStyle.Success);

        if (channelGifSource.HasFlag(GifSourceKind.Klipy))
            klipyButton.WithStyle(ButtonStyle.Success);

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithTextDisplay("## Set Gif Sources");

        var chunkedButtons = buttons.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        return componentBuilder
            .WithSeparator()
            .WithActionRow([backButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls)
            .Build();
    }

    public MessageComponent BuildGiphyRatingToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null)
    {
        var effectiveRating = channelSettings.GiphyRating ?? "pg";

        var buttons = new List<ButtonBuilder>
        {
            new ButtonBuilder()
                .WithCustomId(InteractionId.GiphyRatingGButton)
                .WithLabel("G")
                .WithStyle(ButtonStyle.Primary),
            new ButtonBuilder()
                .WithCustomId(InteractionId.GiphyRatingPgButton)
                .WithLabel("PG")
                .WithStyle(ButtonStyle.Primary),
            new ButtonBuilder()
                .WithCustomId(InteractionId.GiphyRatingPg13Button)
                .WithLabel("PG-13")
                .WithStyle(ButtonStyle.Primary),
            new ButtonBuilder()
                .WithCustomId(InteractionId.GiphyRatingRButton)
                .WithLabel("R")
                .WithStyle(ButtonStyle.Primary),
            new ButtonBuilder()
                .WithCustomId(InteractionId.GiphyRatingAllButton)
                .WithLabel("All")
                .WithStyle(ButtonStyle.Primary)
        };

        var selected = selectedValue;

        if (selected is null)
        {
            if (effectiveRating == "g")
                selected = InteractionId.GiphyRatingGButton;
            else if (effectiveRating == "pg")
                selected = InteractionId.GiphyRatingPgButton;
            else if (effectiveRating == "pg-13")
                selected = InteractionId.GiphyRatingPg13Button;
            else if (effectiveRating == "r")
                selected = InteractionId.GiphyRatingRButton;
            else if (effectiveRating == "all")
                selected = InteractionId.GiphyRatingAllButton;
            else
                selected = InteractionId.GiphyRatingPgButton;
        }

        var buttonsArray = buttons.ToArray();

        foreach (var button in buttonsArray)
        {
            var customId = button.CustomId;

            if (customId == selected)
                button.WithStyle(ButtonStyle.Success);
        }

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithTextDisplay("## Set Giphy Rating");

        var chunkedButtons = buttonsArray.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        return componentBuilder
            .WithSeparator()
            .WithActionRow([backButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls)
            .Build();
    }

    public MessageComponent BuildHowOftenToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null)
    {
        var buttons = new List<ButtonBuilder>();

        foreach (var minute in _appConfig.CurrentValue.Intervals.Minutes)
        {
            var unit = minute == 1 ? "Minute" : "Minutes";
            var value = $"{minute}-{(int)Interval.Minutes}";
            var label = $"{minute} {unit}";

            buttons.Add(
                new ButtonBuilder()
                    .WithCustomId($"{InteractionId.HowOftenButtonPrefix}:{value}")
                    .WithLabel(label)
                    .WithStyle(ButtonStyle.Primary)
            );
        }

        foreach (var hour in _appConfig.CurrentValue.Intervals.Hours)
        {
            var unit = hour == 1 ? "Hour" : "Hours";
            var value = $"{hour}-{(int)Interval.Hours}";
            var label = $"{hour} {unit}";

            buttons.Add(
                new ButtonBuilder()
                    .WithCustomId($"{InteractionId.HowOftenButtonPrefix}:{value}")
                    .WithLabel(label)
                    .WithStyle(ButtonStyle.Primary)
            );
        }

        var neverValue = $"0-{(int)Interval.None}";

        buttons.Add(
            new ButtonBuilder()
                .WithCustomId($"{InteractionId.HowOftenButtonPrefix}:{neverValue}")
                .WithLabel("Never")
                .WithStyle(ButtonStyle.Primary)
        );

        var selected = selectedValue ?? (channelSettings.Frequency == 0 || channelSettings.Interval == Interval.None
            ? neverValue
            : $"{channelSettings.Frequency}-{(int)channelSettings.Interval}");

        var selectedButtonId = $"{InteractionId.HowOftenButtonPrefix}:{selected}";

        var buttonsArray = buttons.ToArray();

        foreach (var button in buttonsArray)
        {
            var customId = button.CustomId;

            if (customId == selectedButtonId)
                button.WithStyle(ButtonStyle.Success);
        }

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithTextDisplay("## Post Gifs How Often? Every:");

        var chunkedButtons = buttonsArray.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        return componentBuilder
            .WithSeparator()
            .WithActionRow([backButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls)
            .Build();
    }

    public MessageComponent BuildPostingBehaviorToggleModal(ChannelSettingsDto channelSettings, string channelName, string? selectedValue = null)
    {
        var gifPostingBehaviorKind = channelSettings.GifPostingBehavior;

        var trendingOnlyButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorTrendingOnlyButton)
            .WithLabel("Post Trending Gifs Only")
            .WithStyle(ButtonStyle.Primary);

        var trendingWithRandomButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorTrendingWithRandomButton)
            .WithLabel("Post Random Gifs When There's No New Trending Gifs")
            .WithStyle(ButtonStyle.Primary);

        var selected = selectedValue ?? (gifPostingBehaviorKind == GifPostingBehaviorKind.TrendingGifsOnly
            ? InteractionId.PostingBehaviorTrendingOnlyButton
            : InteractionId.PostingBehaviorTrendingWithRandomButton);

        var buttonsArray = new[] { trendingOnlyButton, trendingWithRandomButton };

        foreach (var button in buttonsArray)
        {
            var customId = button.CustomId;

            if (customId == selected)
                button.WithStyle(ButtonStyle.Success);
        }

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay("# Trending Gif Bot")
            .WithTextDisplay("## Set Posting Behavior");

        var chunkedButtons = buttonsArray.Chunk(5);

        foreach (var chunk in chunkedButtons)
            componentBuilder = componentBuilder.WithActionRow(chunk);

        var backButton = new ButtonBuilder()
            .WithCustomId(InteractionId.BackButton)
            .WithLabel("Back")
            .WithStyle(ButtonStyle.Secondary);

        return componentBuilder
            .WithSeparator()
            .WithActionRow([backButton])
            .WithSeparator()
            .WithMediaGallery(_appConfig.CurrentValue.Attribution.Urls)
            .Build();
    }
}
