using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class ConfirmationCancellationInteractionModule
(
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    IPendingConfirmationManager _confirmationManager
) : BothHooksDtoInteractionModuleBase(_dtoBuilder)
{
    [ComponentInteraction(InteractionId.CancelClearButton)]
    public Task CancelClearAsync()
    {
        _ = _confirmationManager.TryRemove(Context.Channel.Id, out _);

        Component = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }
}
