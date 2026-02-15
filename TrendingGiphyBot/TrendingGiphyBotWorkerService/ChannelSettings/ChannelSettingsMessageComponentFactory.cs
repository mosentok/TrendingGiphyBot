using Discord;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Discord.Interactions;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsMessageComponentFactory(IOptionsMonitor<AppConfig> _appConfig, IUtcOffsetParser _utcOffsetParser) : IChannelSettingsMessageComponentFactory
{
    public MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName)
    {
        var neverBuilder = new SelectMenuOptionBuilder()
            .WithLabel("Never")
            .WithValue($"0-{(int)IntervalDescription.None}");

        var minutesBuilders = _appConfig.CurrentValue.Intervals.Minutes.Select(static minute =>
            new SelectMenuOptionBuilder()
                .WithLabel($"Post Gifs Every {minute} Minutes")
                .WithValue($"{minute}-{(int)IntervalDescription.Minutes}"));

        // this is a big assumption that 1 hour is both configured and the first item in the list
        var hour1Builder = new SelectMenuOptionBuilder()
            .WithLabel("Post Gifs Every 1 Hour")
            .WithValue($"1-{(int)IntervalDescription.Hours}");

        var hoursBuilders = _appConfig.CurrentValue.Intervals.Hours.Skip(1).Select(static hour =>
            new SelectMenuOptionBuilder()
                .WithLabel($"Post Gifs Every {hour} Hours")
                .WithValue($"{hour}-{(int)IntervalDescription.Hours}"));

        var howOftenOptions = new[] { neverBuilder }.Concat(minutesBuilders).Append(hour1Builder).Concat(hoursBuilders).ToList();

        var selectedOption = channelSettings.IntervalId == (int)IntervalDescription.None
            ? neverBuilder
            : howOftenOptions.Single(s => s.Value == $"{channelSettings.Frequency}-{channelSettings.IntervalId}");

        selectedOption.IsDefault = true;

        var howOftenSelectMenu = new SelectMenuBuilder()
            .WithCustomId(InteractionId.HowOftenSelectMenu)
            .WithPlaceholder("How often gifs get posted")
            .WithOptions(howOftenOptions);

        var (gifsOnlyButtonLabel, gifsOnlyButtonStyle, randomGifsButtonLabel, randomGifsButtonStyle) = (GifPostingBehaviorKind)channelSettings.GifPostingBehaviorId switch
        {
            GifPostingBehaviorKind.TrendingGifsWithRandomGifs => ("Post Trending Gifs Only", ButtonStyle.Primary, "=> Post Random Gifs When There's No New Trending Gifs <=", ButtonStyle.Success),
            GifPostingBehaviorKind.TrendingGifsOnly or _ => ("=> Post Trending Gifs Only <=", ButtonStyle.Success, "Post Random Gifs When There's No New Trending Gifs", ButtonStyle.Primary)
        };

        var trendingGifsOnlyButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsOnlyButton)
            .WithLabel(gifsOnlyButtonLabel)
            .WithStyle(gifsOnlyButtonStyle);

        var trendingGifsWithRandomButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithRandomButton)
            .WithLabel(randomGifsButtonLabel)
            .WithStyle(randomGifsButtonStyle);

        var gifKeywordButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingGifsWithKeywordModalButton)
            .WithLabel("Set Random Gif Keywords")
            .WithStyle(ButtonStyle.Secondary);

        var clearGifKeywordButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearKeywordModalButton)
            .WithLabel($"""Clear Random Gif Keywords (Currently "{channelSettings.GifKeyword ?? "<none>"}")""")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.GifKeyword is null);

        var setPostingHoursButton = new ButtonBuilder()
            .WithCustomId(InteractionId.TrendingPostingHoursModalButton)
            .WithLabel("Set Posting Hours")
            .WithStyle(ButtonStyle.Secondary);

        var postingHoursDisplay = DeterminePostingHoursDisplay();

        var clearPostingHoursButton = new ButtonBuilder()
            .WithCustomId(InteractionId.ClearPostingHoursModalButton)
            .WithLabel($"""Clear Posting Hours (Currently "{postingHoursDisplay}")""")
            .WithStyle(ButtonStyle.Danger)
            .WithDisabled(channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null);

        var allGifSources = Enum.GetValues<GifSourceKind>();

        var channelGifSource = channelSettings.GifSource ?? allGifSources.Aggregate((left, right) => left | right);

        var gifSourcesBuilders = allGifSources
            .Except([GifSourceKind.None])
            .Select(gifSource =>
            {
                var gifSourceString = gifSource.ToString();

                return new SelectMenuOptionBuilder()
                    .WithLabel(gifSourceString)
                    .WithValue(gifSourceString)
                    .WithDefault(channelGifSource.HasFlag(gifSource));
            })
            .ToList();

        var gifSourcesSelectMenu = new SelectMenuBuilder()
            .WithCustomId(InteractionId.GifSourcesSelectMenu)
            .WithPlaceholder("Which gif sources to use")
            .WithOptions(gifSourcesBuilders)
            .WithMinValues(0)
            .WithMaxValues(2);

        var gRatingOptionBuilder = new SelectMenuOptionBuilder()
            .WithLabel("G")
            .WithValue("g")
            .WithDefault(channelSettings.GiphyRating == "g");

        var pgRatingOptionBuilder = new SelectMenuOptionBuilder()
            .WithLabel("PG")
            .WithValue("pg")
            .WithDefault(channelSettings.GiphyRating is null or "pg");

        var pg13RatingOptionBuilder = new SelectMenuOptionBuilder()
            .WithLabel("PG-13")
            .WithValue("pg-13")
            .WithDefault(channelSettings.GiphyRating == "pg-13");

        var rRatingOptionBuilder = new SelectMenuOptionBuilder()
            .WithLabel("R")
            .WithValue("r")
            .WithDefault(channelSettings.GiphyRating == "r");

        var allRatingOptionBuilder = new SelectMenuOptionBuilder()
            .WithLabel("All")
            .WithValue("all")
            .WithDefault(channelSettings.GiphyRating == "all");

        var giphyRatingSelectMenu = new SelectMenuBuilder()
            .WithCustomId(InteractionId.GiphyRatingSelectMenu)
            .WithPlaceholder("Giphy rating (Klipy doesn't support ratings)")
            .WithOptions(
            [
                gRatingOptionBuilder,
                pgRatingOptionBuilder,
                pg13RatingOptionBuilder,
                rRatingOptionBuilder,
                allRatingOptionBuilder
            ]);

        return new ComponentBuilderV2()
            .WithTextDisplay($"# Settings for: **{channelName}**")
            .WithSeparator()
            .WithTextDisplay("## Main Settings")
            .WithActionRow([howOftenSelectMenu])
            .WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton])
            .WithSeparator()
            .WithTextDisplay("## Optional Settings")
            .WithTextDisplay("### Gif sources")
            .WithActionRow([gifSourcesSelectMenu])
            .WithTextDisplay("### Giphy rating")
            .WithTextDisplay("-# (Klipy doesn't support rating selection)")
            .WithActionRow([giphyRatingSelectMenu])
            .WithActionRow([gifKeywordButton, clearGifKeywordButton])
            .WithActionRow([setPostingHoursButton, clearPostingHoursButton])
            .WithSeparator()
            .WithMediaGallery(["attachment://PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif", "attachment://Powered by KLIPY Horizontal - Yellow&White Logo.png"])
            .Build();

        string DeterminePostingHoursDisplay()
        {
            if (channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null)
                return "<none>";

            if (channelSettings.UtcOffset is null or "")
                return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo}";

            var formattedUtcOffset = _utcOffsetParser.FormatUtcOffsetString(channelSettings.UtcOffset);

            return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo} ({formattedUtcOffset})";
        }
    }
}
