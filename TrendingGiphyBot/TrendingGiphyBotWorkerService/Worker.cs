using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService;

public class Worker(ILoggerWrapper<Worker> _loggerWrapper) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			await Task.Delay(-1, stoppingToken);
		}
		catch (Exception exception)
		{
			_loggerWrapper.LogTopLevelException(exception);
		}
	}
}

[Group("tgb", "Trending Giphy Bot commands for your server")]
public class TgbInteractionModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
	static readonly List<SelectMenuOptionBuilder> minutesBuilders =
	[
		//minutes
		.. new[] { 5, 10, 15, 30 }
			.Select(minute =>
				new SelectMenuOptionBuilder()
					.WithLabel($"Every {minute} minutes")
					.WithValue($"every-{minute}-minutes")),

		//hours
		.. new[] {1, 2, 3, 4, 6, 8, 12, 24 }
			.Select(hour =>
			{
				//conditionally pluralize the word "hour" in the labels and values
				var (label, value) = hour == 1
					? ($"Every {hour} hour", $"every-{hour}-hour")
					: ($"Every {hour} hours", $"every-{hour}-hours");

				return new SelectMenuOptionBuilder()
					.WithLabel(label)
					.WithValue(value);
		})
	];

	[SlashCommand("settings", "View and change your Trending Giphy Bot's settings for your server")]
	public async Task SettingsAsync()
	{
		var howOftenSelectMenu = new SelectMenuBuilder()
			.WithCustomId("how-often-select-menu")
			.WithPlaceholder("How often gifs get posted")
			.WithOptions(minutesBuilders);

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
			.WithRequired(false);

		var components = new ComponentBuilderV2()
			.WithTextDisplay("# Trending Giphy Bot Settings")
			.WithSeparator()
			.WithTextDisplay("## How often gifs get posted")
			.WithTextDisplay("Current value: TODO")
			.WithActionRow([howOftenSelectMenu])
			.WithSeparator()
			.WithTextDisplay("## Gif posting behavior")
			.WithTextDisplay("Current value: TODO")
			.WithActionRow([trendingGifsOnlyButton, trendingGifsWithRandomButton, trendingGifsWithKeywordButton])
			.WithSeparator(isDivider: false)
			.WithTextDisplay("## Keyword to post gifs of when up-to-date")
			.WithTextDisplay("Current value: TODO")
			.WithActionRow([gifKeywordTextInput])
			.WithSeparator()
			.WithTextDisplay("## Posting hours")
			.WithTextDisplay("Current value: TODO")
			.WithActionRow([])
			.WithSeparator(isDivider: false)
			.WithTextDisplay("## Timezone (UTC)")
			.WithTextDisplay("Current value: TODO")
			.WithActionRow([])
			.WithSeparator()
			.Build();

		await RespondAsync(components: components);
	}

	[ComponentInteraction("custom_id")]
	public async Task Command()
	{
		await Context.Interaction.UpdateAsync(messageProperties =>
		{
			if (!messageProperties.Components.IsSpecified)
				return;
		});
	}
	//[Group("get", "Get Trending Giphy Bot's options for your server")]
	//public class GetInteractionModule : InteractionModuleBase
	//{
	//	[SlashCommand("options", "Get Trending Giphy Bot's options for your server")]
	//	public async Task GetOptionsAsync()
	//	{

	//	}
	//}

	//[Group("set", "Set Trending Giphy Bot's options for your server")]
	//public class SetInteractionModule : InteractionModuleBase
	//{
	//	[SlashCommand("timezone", "Set the timezone of Trending Giphy Bot for your server")]
	//	public async Task SetTimezoneAsync()
	//	{

	//	}
	//}
}
public enum ServerOption
{

}