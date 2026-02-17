using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public abstract class BothHooksDtoInteractionModuleBase
(
    IChannelSettingsDtoBuilder _dtoBuilder
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    ChannelSettingsDto? _channelSettingsDto;

    protected ChannelSettingsDto ChannelSettingsDto => _channelSettingsDto!;

    protected MessageComponent? Component { get; set; }

    public override async Task BeforeExecuteAsync(ICommandInfo command) =>
        _channelSettingsDto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        ThisShouldBeImpossibleException.ThrowIf(Component is null);

        await Context.Interaction.UpdateAsync(messageProperties => messageProperties.Components = Component);
    }
}
