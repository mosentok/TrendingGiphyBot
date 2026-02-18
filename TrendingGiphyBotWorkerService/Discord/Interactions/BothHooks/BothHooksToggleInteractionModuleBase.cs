using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public abstract class BothHooksToggleInteractionModuleBase
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    ChannelSettingsModel? _channelSettingsModel;

    protected ITrendingGiphyBotDbContext TrendingGiphyBotContext => _trendingGiphyBotContext;

    // TODO make this an auto-property
    protected ChannelSettingsModel ChannelSettingsModel => _channelSettingsModel!;

    protected IChannelSettingsMessageComponentFactory SettingsMessageComponentFactory => _settingsMessageComponentFactory;

    public override async Task BeforeExecuteAsync(ICommandInfo command) =>
        _channelSettingsModel = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        var dto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);
        var component = BuildToggleModal(dto, Context.Channel.Name);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = component);
    }

    protected abstract MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName);
}
