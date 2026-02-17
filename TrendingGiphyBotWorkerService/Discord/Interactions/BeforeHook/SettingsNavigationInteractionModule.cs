using Discord.Interactions;
using Discord.WebSocket;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BeforeHook;

public class SettingsNavigationInteractionModule
(
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsInteractionUpdater _interactionUpdater
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction(InteractionId.BackButton)]
    public async Task BackAsync()
    {
        var channelSettingsDto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

        await _interactionUpdater.RefreshInteractionAsync(channelSettingsDto, Context.Channel.Name, Context.Interaction);
    }
}
