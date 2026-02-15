using Discord;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Discord.Interactions.ToggleModal;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsMessageComponentFactory(
    IOptionsMonitor<AppConfig> _appConfig,
    IUtcOffsetParser _utcOffsetParser,
    IToggleModalComponentBuilder _toggleModal
) : IChannelSettingsMessageComponentFactory
{
    public MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName)
    {
        var howOftenButton = new ButtonBuilder()
            .WithCustomId(InteractionId.HowOftenOpenButton)
            .WithLabel("Set How Often to Post Gifs")
            .WithStyle(ButtonStyle.Primary);

        var resetHowOftenButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetHowOftenButton)
            .WithLabel(TrimLabelTo80Chars($"Reset How Often ({DetermineHowOftenDisplay(channelSettings)})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.IntervalId == (int)IntervalDescription.None);

        var postingBehaviorButton = new ButtonBuilder()
            .WithCustomId(InteractionId.PostingBehaviorOpenButton)
            .WithLabel("Set Posting Behavior")
            .WithStyle(ButtonStyle.Primary);

        var resetPostingBehaviorButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetPostingBehaviorButton)
            .WithLabel(TrimLabelTo80Chars($"Reset Posting Behavior ({DeterminePostingBehaviorDisplay(channelSettings)})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(false);

        var gifSourcesButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifSourcesOpenButton)
            .WithLabel("Set Gif Sources")
            .WithStyle(ButtonStyle.Primary);

        var clearGifSourcesButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearGifSourcesButton)
            .WithLabel(TrimLabelTo80Chars($"Clear Gif Sources ({DetermineGifSourceDisplay(channelSettings)})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(false);

        var gifRetentionButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GifRetentionOpenButton)
            .WithLabel("Set Retention Period")
            .WithStyle(ButtonStyle.Primary);

        var resetGifRetentionButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetRetentionPeriodButton)
            .WithLabel(TrimLabelTo80Chars($"Reset Retention Period ({DetermineGifRetentionDisplay(channelSettings)})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(false);

        var giphyRatingButton = new ButtonBuilder()
            .WithCustomId(InteractionId.GiphyRatingOpenButton)
            .WithLabel("Set Giphy Rating")
            .WithStyle(ButtonStyle.Primary);

        var resetGiphyRatingButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ResetGiphyRatingButton)
            .WithLabel(TrimLabelTo80Chars($"Reset Giphy Rating ({DetermineGiphyRatingDisplay(channelSettings)})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(false);

        var gifKeywordButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithKeywordButton)
            .WithLabel("Set Random Gif Keywords")
            .WithStyle(ButtonStyle.Primary);

        var clearGifKeywordButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearKeywordButton)
            .WithLabel(TrimLabelTo80Chars($"Clear Random Gif Keywords ({(channelSettings.GifKeyword ?? "<none>")})"  ))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.GifKeyword is null);

        var setPostingHoursButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingPostingHoursButton)
            .WithLabel("Set Posting Hours")
            .WithStyle(ButtonStyle.Primary);

        var postingHoursDisplay = DeterminePostingHoursDisplay(channelSettings);

        var clearPostingHoursButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearPostingHoursButton)
            .WithLabel(TrimLabelTo80Chars($"Clear Posting Hours ({postingHoursDisplay})"))
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null);

        var componentBuilder = new ComponentBuilderV2()
            .WithTextDisplay($"# Settings for: **{channelName}**")
            .WithSeparator()
            .WithTextDisplay("## Main Settings")
            .WithActionRow([howOftenButton, resetHowOftenButton])
            .WithActionRow([postingBehaviorButton, resetPostingBehaviorButton])
            .WithSeparator()
            .WithTextDisplay("## Optional Settings")
            .WithActionRow([gifSourcesButton, clearGifSourcesButton])
            .WithActionRow([gifRetentionButton, resetGifRetentionButton])
            .WithActionRow([giphyRatingButton, resetGiphyRatingButton])
            .WithActionRow([gifKeywordButton, clearGifKeywordButton])
            .WithActionRow([setPostingHoursButton, clearPostingHoursButton])
            .WithSeparator()
            .WithMediaGallery([
                "attachment://PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif",
                "attachment://Powered by KLIPY Horizontal - Yellow&White Logo.png"
            ]);

        return componentBuilder.Build();
    }

    public MessageComponent BuildHowOftenToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null)
    {
        var buttons = new List<ButtonBuilder>();

        foreach (var minute in _appConfig.CurrentValue.Intervals.Minutes)
        {
            var unit = minute == 1 ? "Minute" : "Minutes";
            var value = $"{minute}-{(int)IntervalDescription.Minutes}";
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
            var value = $"{hour}-{(int)IntervalDescription.Hours}";
            var label = $"{hour} {unit}";

            buttons.Add(
                new ButtonBuilder()
                    .WithCustomId($"{InteractionId.HowOftenButtonPrefix}:{value}")
                    .WithLabel(label)
                    .WithStyle(ButtonStyle.Primary)
            );
        }

        var neverValue = $"0-{(int)IntervalDescription.None}";

        buttons.Add(
            new ButtonBuilder()
                .WithCustomId($"{InteractionId.HowOftenButtonPrefix}:{neverValue}")
                .WithLabel("Never")
                .WithStyle(ButtonStyle.Primary)
        );

        var selected = selectedValue ?? (channelSettings.Frequency == 0 || channelSettings.IntervalId == (int)IntervalDescription.None
            ? neverValue
            : $"{channelSettings.Frequency}-{channelSettings.IntervalId}");

        var selectedButtonId = $"{InteractionId.HowOftenButtonPrefix}:{selected}";

        var buttonsArray = buttons.ToArray();
        
        ApplySelectedStyle(buttonsArray, selectedButtonId);

        return _toggleModal.BuildToggleModal("Post Gifs How Often? Every:", buttonsArray);
    }

    public MessageComponent BuildPostingBehaviorToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null)
    {
        var gifPostingBehaviorKind = (GifPostingBehaviorKind)channelSettings.GifPostingBehaviorId;

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
        
        ApplySelectedStyle(buttonsArray, selected);

        return _toggleModal.BuildToggleModal("Set Posting Behavior", buttonsArray);
    }

    public MessageComponent BuildGifSourcesToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null)
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

        return _toggleModal.BuildToggleModal("Set Gif Sources", buttons);
    }

    public MessageComponent BuildGifRetentionToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null)
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
        
        ApplySelectedStyle(buttonsArray, selectedButtonId);

        return _toggleModal.BuildToggleModal("Set Retention Period", buttonsArray);
    }

    public MessageComponent BuildGiphyRatingToggleModal(ChannelSettingsModel channelSettings, string? selectedValue = null)
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
        
        ApplySelectedStyle(buttonsArray, selected);

        return _toggleModal.BuildToggleModal("Set Giphy Rating", buttonsArray);
    }

    private string TrimLabelTo80Chars(string label)
    {
        const int maxLength = 80;

        if (label.Length <= maxLength)
            return label;

        return label[..(maxLength - 3)] + "...";
    }

    private string DeterminePostingHoursDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null)
            return "<none>";

        if (channelSettings.UtcOffset is null or "")
            return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo}";

        var formattedUtcOffset = _utcOffsetParser.FormatUtcOffsetString(channelSettings.UtcOffset);

        return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo} ({formattedUtcOffset})";
    }

    private string DetermineHowOftenDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.IntervalId == (int)IntervalDescription.None || channelSettings.Frequency == 0)
            return "Never";

        var intervalDescription = (IntervalDescription)channelSettings.IntervalId;

        return intervalDescription switch
        {
            IntervalDescription.Minutes =>
                channelSettings.Frequency == 1 ? "Every 1 Minute" : $"Every {channelSettings.Frequency} Minutes",
            IntervalDescription.Hours =>
                channelSettings.Frequency == 1 ? "Every 1 Hour" : $"Every {channelSettings.Frequency} Hours",
            _ => "<unknown>"
        };
    }

    private string DeterminePostingBehaviorDisplay(ChannelSettingsModel channelSettings)
    {
        var behaviorKind = (GifPostingBehaviorKind)channelSettings.GifPostingBehaviorId;

        return behaviorKind switch
        {
            GifPostingBehaviorKind.TrendingGifsOnly => "Trending Only",
            GifPostingBehaviorKind.TrendingGifsWithRandomGifs => "Trending With Random",
            _ => "<unknown>"
        };
    }

    private string DetermineGifSourceDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.GifSource is null)
            return "<none>";

        var sources = new List<string>();

        if ((channelSettings.GifSource & GifSourceKind.Giphy) != 0)
            sources.Add("Giphy");

        if ((channelSettings.GifSource & GifSourceKind.Klipy) != 0)
            sources.Add("Klipy");

        return sources.Count == 0 ? "<none>" : string.Join(" & ", sources);
    }

    private string DetermineGifRetentionDisplay(ChannelSettingsModel channelSettings)
    {
        var retentionDays = channelSettings.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;
        return retentionDays == 1 ? "1 day" : $"{retentionDays} days";
    }

    private string DetermineGiphyRatingDisplay(ChannelSettingsModel channelSettings)
    {
        var rating = channelSettings.GiphyRating ?? "pg";

        return rating.ToLower() switch
        {
            "g" => "G",
            "pg" => "PG",
            "pg-13" => "PG-13",
            "r" => "R",
            "all" => "All",
            _ => "<unknown>"
        };
    }

    public MessageComponent BuildConfirmationModal(string message)
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
            .WithTextDisplay(message)
            .WithActionRow([confirmButton, cancelButton])
            .WithSeparator()
            .WithMediaGallery([
                "attachment://PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif",
                "attachment://Powered by KLIPY Horizontal - Yellow&White Logo.png"
            ]);

        return componentBuilder.Build();
    }

    private void ApplySelectedStyle(ButtonBuilder[] buttons, string selectedCustomId)
    {
        foreach (var button in buttons)
        {
            var customId = button.CustomId;
            
            if (customId == selectedCustomId)
                button.WithStyle(ButtonStyle.Success);
        }
    }}