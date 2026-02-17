using Discord.Interactions;
using Discord.WebSocket;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.NoHooks;

public class ConfirmationCancellationInteractionModule
(
    IPendingConfirmationManager _confirmationManager
) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction(InteractionId.CancelClearButton)]
    public async Task CancelClearAsync() =>
        _confirmationManager.TryRemove(Context.Channel.Id, out _);
}
