using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public abstract class BothHooksInteractionModuleBase
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsInteractionUpdater _interactionUpdater
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    ChannelSettingsModel? _channelSettingsModel;

    protected ITrendingGiphyBotDbContext TrendingGiphyBotContext => _trendingGiphyBotContext;

    protected ChannelSettingsModel ChannelSettingsModel => _channelSettingsModel!;

    public override async Task BeforeExecuteAsync(ICommandInfo command) =>
        _channelSettingsModel = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        var dto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

        await _interactionUpdater.RefreshInteractionAsync(dto, Context.Channel.Name, Context.Interaction);
    }
}
