using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService
{
    public class ModalSubmittingInteractionModule(IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory, ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        [ModalInteraction("trending-gifs-with-keyword-modal")]
        public async Task SetKeywordAsync(FeedbackModal modal)
        {
            var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

            channelSettings.GifKeyword = modal.Keyword;

            await _trendingGiphyBotContext.SaveChangesAsync();

            var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings);

            await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
        }
    }
}