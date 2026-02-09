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

        //var giphyOptionBuilder = new SelectMenuOptionBuilder()
        //    .WithLabel(nameof(GifSourceKind.Giphy))
        //    .WithValue(nameof(GifSourceKind.Giphy));

        //var klipyOptionBuilder = new SelectMenuOptionBuilder()
        //    .WithLabel(nameof(GifSourceKind.Klipy))
        //    .WithValue(nameof(GifSourceKind.Klipy));

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

        //var allOptionBuilder = new SelectMenuOptionBuilder()
        //    .WithLabel(allGifSources)
        //    .WithValue(allGifSources);

        //var gifSourcesBuilders = new List<SelectMenuOptionBuilder>
        //{
        //    giphyOptionBuilder,
        //    klipyOptionBuilder,
        //    allOptionBuilder
        //};

        //var selectedGifSources = channelSettings.GifSource is { } gifSource
        //    ? gifSourcesBuilders.Where(s => s.Value == channelGifSource.ToString())
        //    : [allOptionBuilder];

        //selectedGifSources.IsDefault = true;

        var gifSourcesSelectMenu = new SelectMenuBuilder()
            .WithCustomId(InteractionId.GifSourcesSelectMenu)
            .WithPlaceholder("Which gif sources to use")
            .WithOptions(gifSourcesBuilders)
            .WithMinValues(0)
            .WithMaxValues(2);

        return new ComponentBuilderV2()
            .WithTextDisplay($"# Trending Giphy Bot Settings for: **{channelName}**")
            .WithSeparator()
            .WithTextDisplay("## Main Settings")
            .WithActionRow([howOftenSelectMenu])
            .WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton])
            .WithSeparator()
            .WithTextDisplay("## Optional Settings")
            .WithActionRow([gifKeywordButton, clearGifKeywordButton])
            .WithActionRow([gifSourcesSelectMenu])
            .WithActionRow([setPostingHoursButton, clearPostingHoursButton])
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
