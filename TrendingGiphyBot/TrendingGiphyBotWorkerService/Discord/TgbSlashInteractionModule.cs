using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord;

[Group("tgb", "Trending Giphy Bot commands for your server")]
public class TgbSlashInteractionModule(IChannelSettingsMessageComponentFactory _channelSettingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("settings", "View and change your Trending Giphy Bot's settings for this channel")]
	public async Task GetOrCreateChannelSettingsAsync()
	{
		var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleOrDefaultAsync(s => s.ChannelId == Context.Channel.Id);

		if (channelSettings is null)
		{
			channelSettings = new() { ChannelId = Context.Channel.Id };

			_trendingGiphyBotContext.ChannelSettings.Add(channelSettings);

			await _trendingGiphyBotContext.SaveChangesAsync();
		}

		var channelSettingsMessageComponent = _channelSettingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, Context.Channel.Name);

		await RespondAsync(components: channelSettingsMessageComponent);
	}
}