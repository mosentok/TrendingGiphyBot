using Discord;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsMessageComponentFactory(IntervalConfig _intervalConfig) : IChannelSettingsMessageComponentFactory
{
	public MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName)
	{
        var neverBuilder = new SelectMenuOptionBuilder()
			.WithLabel("Never")
			.WithValue($"0-{nameof(IntervalDescription.None)}");

		var minutesBuilders = _intervalConfig.Minutes.Select(static minute =>
			new SelectMenuOptionBuilder()
				.WithLabel($"Post Gifs Every {minute} Minutes")
				.WithValue($"{minute}-{nameof(IntervalDescription.Minutes)}"));

		// this is a big assumption that 1 hour is both configured and the first item in the list
		var hour1Builder = new SelectMenuOptionBuilder()
			.WithLabel("Post Gifs Every 1 Hour")
			.WithValue($"1-{nameof(IntervalDescription.Hours)}");

		var hoursBuilders = _intervalConfig.Hours.Skip(1).Select(static hour =>
			new SelectMenuOptionBuilder()
				.WithLabel($"Post Gifs Every {hour} Hours")
				.WithValue($"{hour}-{nameof(IntervalDescription.Hours)}"));

		var howOftenOptions = new[] { neverBuilder }.Concat(minutesBuilders).Append(hour1Builder).Concat(hoursBuilders).ToList();

        var intervalDescription = (IntervalDescription)channelSettings.IntervalId;

        var selectedOption = intervalDescription == IntervalDescription.None
			? neverBuilder
            : howOftenOptions.Single(s => s.Value == $"{channelSettings.Frequency}-{intervalDescription}");

        selectedOption.IsDefault = true;

		var howOftenSelectMenu = new SelectMenuBuilder()
			.WithCustomId("how-often-select-menu")
			.WithPlaceholder("How often gifs get posted")
			.WithOptions(howOftenOptions);

		var (gifsOnlyButtonLabel, gifsOnlyButtonStyle, randomGifsButtonLabel, randomGifsButtonStyle) = channelSettings.GifPostingBehavior switch
		{
			"trending-gifs-with-random-button" => ("Post Trending Gifs Only", ButtonStyle.Primary, "=> Post Random Gifs When There's No New Trending Gifs <=", ButtonStyle.Success),
			"trending-gifs-only-button" or _ => ("=> Post Trending Gifs Only <=", ButtonStyle.Success, "Post Random Gifs When There's No New Trending Gifs", ButtonStyle.Primary)
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
			.WithLabel("(Optional) Set Random Gif Keywords")
			.WithStyle(ButtonStyle.Secondary);

		var clearGifKeywordButton = new ButtonBuilder()
			.WithCustomId("clear-keyword-modal-button")
			.WithLabel($"""Clear Random Gif Keywords (Currently "{channelSettings.GifKeyword ?? "<none>"}")""")
			.WithStyle(ButtonStyle.Danger)
			.WithDisabled(channelSettings.GifKeyword is null);

		var setPostingHoursButtonLabel = channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null || channelSettings.TimeZone is null
			? "<none>"
			: $"{channelSettings.PostingHoursFrom} - {channelSettings.PostingHoursTo} {channelSettings.TimeZone}";

		var setPostingHoursButton = new ButtonBuilder()
			.WithCustomId("set-posting-hours-modal-button")
			.WithLabel($"Set Posting Hours (Currently {setPostingHoursButtonLabel})")
			.WithStyle(ButtonStyle.Secondary);

		return new ComponentBuilderV2()
			.WithTextDisplay("# Trending Giphy Bot Settings")
			.WithSeparator()
			.WithActionRow([howOftenSelectMenu])
			.WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton])
			.WithSeparator()
			.WithActionRow([gifKeywordButton, clearGifKeywordButton, setPostingHoursButton])
			.Build();
	}
}
