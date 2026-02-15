using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[Group("tgb", "Trending Gif Bot commands for this channel")]
public class TgbSlashInteractionModule(IChannelSettingsMessageComponentFactory _channelSettingsMessageComponentFactory, ITrendingGiphyBotDbContext _trendingGiphyBotContext, IOptionsMonitor<AppConfig> _appConfig) : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("settings", "View and change your Trending Gif Bot's settings for this channel")]
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

        var attachments = _appConfig.CurrentValue.Attribution.AttachmentFileNames.Select(fileName => new FileAttachment(fileName)).ToArray();

		await RespondWithFilesAsync(
			attachments,
			components: channelSettingsMessageComponent,
			ephemeral: true);
	}
}