using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord;

namespace TrendingGiphyBotWorkerService.Keywords;

public class KeywordModalInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotDbContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
{
	[ModalInteraction("trending-gifs-with-keyword-modal")]
	public async Task SetKeywordAsync(FeedbackModal modal)
	{
		var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

		channelSettings.GifKeyword = modal.Keyword;

		await _trendingGiphyBotContext.SaveChangesAsync();

		var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, Context.Channel.Name);

		await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
	}
}
