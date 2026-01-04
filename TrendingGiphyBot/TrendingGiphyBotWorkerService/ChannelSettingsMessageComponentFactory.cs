using Discord;

namespace TrendingGiphyBotWorkerService;

public class ChannelSettings
{
	public ulong ChannelId { get; set; }
	public string? HowOften { get; set; }
	public string? GifPostingBehavior { get; set; }
	public string? GifKeyword { get; set; }
	public string? PostingHoursFrom { get; set; }
	public string? PostingHoursTo { get; set; }
	public string? TimeZone { get; set; }
}

public class ChannelSettingsMessageComponentFactory(int[] _minutes, int[] _hours) : IChannelSettingsMessageComponentFactory
{
	public MessageComponent BuildChannelSettingsMessageComponent(ChannelSettings channelSettings)
	{
		var minutesBuilders = _minutes.Select(static minute =>
			new SelectMenuOptionBuilder()
				.WithLabel($"Every {minute} minutes")
				.WithValue($"every-{minute}-minutes"));

		var hour1Builder = new SelectMenuOptionBuilder()
			.WithLabel("Every 1 hour")
			.WithValue("every-1-hour");

		var hoursBuilders = _hours.Select(static hour =>
			new SelectMenuOptionBuilder()
				.WithLabel($"Every {hour} hours")
				.WithValue($"every-{hour}-hours"));

		var howOftenOptions = minutesBuilders.Append(hour1Builder).Concat(hoursBuilders).ToList();

		var howOftenSelectMenu = new SelectMenuBuilder()
			.WithCustomId("how-often-select-menu")
			.WithPlaceholder("How often gifs get posted")
			.WithOptions(howOftenOptions);

		var trendingGifsOnlyButton = new ButtonBuilder()
			.WithCustomId("trending-gifs-only-button")
			.WithLabel("Trending Gifs Only")
			.WithStyle(ButtonStyle.Secondary);

		var trendingGifsWithRandomButton = new ButtonBuilder()
			.WithCustomId("trending-gifs-with-random-button")
			.WithLabel("Trending Gifs + Random Gifs When Up-to-date")
			.WithStyle(ButtonStyle.Secondary);

		var trendingGifsWithKeywordButton = new ButtonBuilder()
			.WithCustomId("trending-gifs-with-keyword-button")
			.WithLabel("Trending Gifs + Keyword Gifs When Up-to-date")
			.WithStyle(ButtonStyle.Secondary);

		var gifKeywordTextInput = new TextInputBuilder()
			.WithCustomId("trending-gifs-with-keyword-text-input")
			.WithPlaceholder("Keyword to post gifs of when up-to-date")
			.WithLabel("Keyword to post gifs of when up-to-date")
			.WithRequired(false);

		var gifKeywordButton = new ButtonBuilder()
			.WithCustomId("trending-gifs-with-keyword-modal-button")
			.WithLabel("Set Keyword")
			.WithStyle(ButtonStyle.Secondary);

		var postingHoursFromTextInput = new TextInputBuilder()
			.WithCustomId("posting-hours-from")
			.WithPlaceholder("From (0-23)")
			.WithLabel("From (0-23)")
			.WithRequired(false);

		var postingHoursToTextInput = new TextInputBuilder()
			.WithCustomId("posting-hours-to")
			.WithPlaceholder("To (0-23)")
			.WithLabel("To (0-23)")
			.WithRequired(false);

		var timeZoneInput = new TextInputBuilder()
			.WithCustomId("time-zone")
			.WithPlaceholder("TimeZone (ex: UTC-5:00, UTC+10:30)")
			.WithLabel("TimeZone (ex: UTC-5:00, UTC+10:30)")
			.WithRequired(false);

		const string none = "<none>";

		//TODO fix bugs
		/*
		UNION_TYPE_CHOICES: Value of field "type" must be one of (2, 3, 5, 6, 7, 8).
		UNION_TYPE_CHOICES: Value of field "type" must be one of (2, 3, 5, 6, 7, 8).
		UNION_TYPE_CHOICES: Value of field "type" must be one of (2, 3, 5, 6, 7, 8).
		UNION_TYPE_CHOICES: Value of field "type" must be one of (2, 3, 5, 6, 7, 8).'
		*/

		return new ComponentBuilderV2()
			.WithTextDisplay("# Trending Giphy Bot Settings")
			.WithSeparator()
			.WithTextDisplay("## How often gifs get posted")
			.WithTextDisplay($"Current value: {channelSettings.HowOften ?? "Every 1 hour"}")
			.WithActionRow([howOftenSelectMenu])
			.WithSeparator()
			.WithTextDisplay("## Gif posting behavior")
			.WithTextDisplay($"Current value: {channelSettings.GifPostingBehavior ?? "Trending Gifs Only"}")
			.WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton, trendingGifsWithKeywordButton])
			.WithSeparator(isDivider: false)
			.WithTextDisplay("## Keyword to post gifs of when up-to-date")
			.WithTextDisplay($"Current value: {channelSettings.GifKeyword ?? none}")
			.WithActionRow([gifKeywordButton])
			.WithSeparator()
			.WithTextDisplay("## Posting hours")
			.WithTextDisplay($"Current value: from {channelSettings.PostingHoursFrom ?? none} to {channelSettings.PostingHoursTo ?? none}")
			//.WithActionRow([postingHoursFromTextInput])
			//.WithActionRow([postingHoursToTextInput])
			.WithSeparator(isDivider: false)
			.WithTextDisplay("## Timezone (UTC)")
			.WithTextDisplay($"Current value: {channelSettings.TimeZone ?? none}")
			//.WithActionRow([timeZoneInput])
			.WithSeparator()
			.Build();
	}
}
