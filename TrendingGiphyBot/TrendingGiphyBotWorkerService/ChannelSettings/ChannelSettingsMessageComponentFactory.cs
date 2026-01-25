using Discord;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsMessageComponentFactory(IntervalConfig _intervalConfig) : IChannelSettingsMessageComponentFactory
{
    public MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName)
    {
        var neverBuilder = new SelectMenuOptionBuilder()
            .WithLabel("Never")
            .WithValue($"0-{(int)IntervalDescription.None}");

        var minutesBuilders = _intervalConfig.Minutes.Select(static minute =>
            new SelectMenuOptionBuilder()
                .WithLabel($"Post Gifs Every {minute} Minutes")
                .WithValue($"{minute}-{(int)IntervalDescription.Minutes}"));

        // this is a big assumption that 1 hour is both configured and the first item in the list
        var hour1Builder = new SelectMenuOptionBuilder()
            .WithLabel("Post Gifs Every 1 Hour")
            .WithValue($"1-{(int)IntervalDescription.Hours}");

        var hoursBuilders = _intervalConfig.Hours.Skip(1).Select(static hour =>
            new SelectMenuOptionBuilder()
                .WithLabel($"Post Gifs Every {hour} Hours")
                .WithValue($"{hour}-{(int)IntervalDescription.Hours}"));

        var howOftenOptions = new[] { neverBuilder }.Concat(minutesBuilders).Append(hour1Builder).Concat(hoursBuilders).ToList();

        var selectedOption = channelSettings.IntervalId == (int)IntervalDescription.None
            ? neverBuilder
            : howOftenOptions.Single(s => s.Value == $"{channelSettings.Frequency}-{channelSettings.IntervalId}");

        selectedOption.IsDefault = true;

        var howOftenSelectMenu = new SelectMenuBuilder()
            .WithCustomId("how-often-select-menu")
            .WithPlaceholder("How often gifs get posted")
            .WithOptions(howOftenOptions);

        var (gifsOnlyButtonLabel, gifsOnlyButtonStyle, randomGifsButtonLabel, randomGifsButtonStyle) = (GifPostingBehaviorKind)channelSettings.GifPostingBehaviorId switch
        {
            GifPostingBehaviorKind.TrendingGifsWithRandomGifs => ("Post Trending Gifs Only", ButtonStyle.Primary, "=> Post Random Gifs When There's No New Trending Gifs <=", ButtonStyle.Success),
            GifPostingBehaviorKind.TrendingGifsOnly or _ => ("=> Post Trending Gifs Only <=", ButtonStyle.Success, "Post Random Gifs When There's No New Trending Gifs", ButtonStyle.Primary)
        };

        var trendingGifsOnlyButton = new ButtonBuilder()
            .WithCustomId("trending-gifs-only-button")
            .WithLabel(gifsOnlyButtonLabel)
            .WithStyle(gifsOnlyButtonStyle);

        var trendingGifsWithRandomButton = new ButtonBuilder()
            .WithCustomId("trending-gifs-with-random-button")
            .WithLabel(randomGifsButtonLabel)
            .WithStyle(randomGifsButtonStyle);

        var gifKeywordButton = new ButtonBuilder()
            .WithCustomId("trending-gifs-with-keyword-modal-button")
            .WithLabel("Set Random Gif Keywords")
            .WithStyle(ButtonStyle.Secondary);

        var clearGifKeywordButton = new ButtonBuilder()
            .WithCustomId("clear-keyword-modal-button")
            .WithLabel($"""Clear Random Gif Keywords (Currently "{channelSettings.GifKeyword ?? "<none>"}")""")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.GifKeyword is null);

        var setPostingHoursButton = new ButtonBuilder()
            .WithCustomId("trending-posting-hours-modal-button")
            .WithLabel("Set Posting Hours")
            .WithStyle(ButtonStyle.Secondary);

        var postingHoursDisplay = DeterminePostingHoursDisplay();

        var clearPostingHoursButton = new ButtonBuilder()
            .WithCustomId("clear-posting-hours-modal-button")
            .WithLabel($"""Clear Posting Hours (Currently "{postingHoursDisplay}")""")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null);

        return new ComponentBuilderV2()
            .WithTextDisplay($"# Trending Giphy Bot Settings for: **{channelName}**")
            .WithSeparator()
            .WithTextDisplay("## Main Settings")
            .WithActionRow([howOftenSelectMenu])
            .WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton])
            .WithSeparator()
            .WithTextDisplay("## Optional Settings")
            .WithActionRow([gifKeywordButton, clearGifKeywordButton])
            .WithActionRow([setPostingHoursButton, clearPostingHoursButton])
            .Build();

        string DeterminePostingHoursDisplay()
        {
            if (channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null)
                return "<none>";

            if (channelSettings.UtcOffset is null or 0)
                return $"{channelSettings.PostingHoursFrom} - {channelSettings.PostingHoursTo}";

            var utcOffsetIndicator = channelSettings.UtcOffset.Value > 0
                ? "+"
                : "-";

            return $"{channelSettings.PostingHoursFrom} - {channelSettings.PostingHoursTo} ({utcOffsetIndicator}{channelSettings.UtcOffset})";
        }
    }
}
