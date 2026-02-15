using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[Group("tgb", "Trending Giphy Bot commands for this channel")]
public class TgbSlashInteractionModule(IChannelSettingsMessageComponentFactory _channelSettingsMessageComponentFactory, ITrendingGiphyBotDbContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("settings", "View and change your Trending Giphy Bot's settings for this channel")]
	public async Task GetOrCreateChannelSettingsAsync()
	{
		var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleOrDefaultAsync(s => s.ChannelId == Context.Channel.Id);

		if (channelSettings is null)
		{
			channelSettings = new() { ChannelId = Context.Channel.Id, IntervalId = (int)IntervalDescription.None };

			_trendingGiphyBotContext.ChannelSettings.Add(channelSettings);

			await _trendingGiphyBotContext.SaveChangesAsync();
		}

		var channelSettingsMessageComponent = _channelSettingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, Context.Channel.Name);

		await RespondWithFilesAsync(
			new[] { new FileAttachment("PoweredBy_200_Horizontal_Light-Backgrounds_With_Logo.gif"), new FileAttachment("Powered by KLIPY Horizontal - Yellow&White Logo.png") },
			components: channelSettingsMessageComponent,
			ephemeral: true);
	}
}