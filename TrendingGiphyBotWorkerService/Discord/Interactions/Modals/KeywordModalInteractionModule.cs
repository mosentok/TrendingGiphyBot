using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class KeywordModalInteractionModule(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder
) : BothHooksModalInteractionModuleBase
{
    [ModalInteraction(InteractionId.TrendingGifsWithKeywordModal)]
    public async Task SetKeywordAsync(KeyboardModal keyboardModal)
    {
        var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

        channelSettings.GifKeyword = keyboardModal.Keyword;

        await _trendingGiphyBotContext.SaveChangesAsync();

        var dto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

        Component = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(dto, Context.Channel.Name);
    }
}
