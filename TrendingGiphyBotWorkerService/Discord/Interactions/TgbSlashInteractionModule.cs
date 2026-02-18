using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[Group("tgb", "Trending Gif Bot commands for this channel")]
public class TgbSlashInteractionModule(
	IChannelSettingsMessageComponentFactory _channelSettingsMessageComponentFactory,
	ITrendingGiphyBotDbContext _trendingGiphyBotContext,
	IChannelSettingsDtoBuilder _dtoBuilder
) : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("settings", "View and change your Trending Gif Bot's settings for this channel")]
	public async Task GetOrCreateChannelSettingsAsync()
	{
		var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleOrDefaultAsync(s => s.ChannelId == Context.Channel.Id);

		if (channelSettings is null)
		{
			channelSettings = new()
			{
				GifPostingBehavior = GifPostingBehaviorKind.TrendingGifsOnly,
				ChannelId = Context.Channel.Id,
				Frequency = 30,
				GifSource = GifSourceKind.Giphy & GifSourceKind.Klipy,
				GiphyRating = "PG",
				Interval = Interval.Minutes,
			};

			_trendingGiphyBotContext.ChannelSettings.Add(channelSettings);

			await _trendingGiphyBotContext.SaveChangesAsync();
		}

		var dto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

		var channelSettingsMessageComponent = _channelSettingsMessageComponentFactory.BuildChannelSettingsMessageComponent(dto, Context.Channel.Name);

		await RespondAsync(
            ephemeral: true,
            components: channelSettingsMessageComponent);
	}
}