using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService
{
    [Group("tgb", "Trending Giphy Bot commands for your server")]
    public class TgbSlashInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext>
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

            var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings);

            await RespondAsync(components: settingsMessageComponent);
		}
	}
}