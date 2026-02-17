using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BeforeHook;

public abstract class BeforeHookModelInteractionModuleBase
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    ChannelSettingsModel? _channelSettingsModel;

    protected ChannelSettingsModel ChannelSettingsModel => _channelSettingsModel!;

    public override async Task BeforeExecuteAsync(ICommandInfo command) =>
        _channelSettingsModel = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);
}
