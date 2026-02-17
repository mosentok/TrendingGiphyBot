namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

public interface IPendingConfirmationManager
{
    Task SetPendingAsync(ulong channelId, PendingClearAction action, string message);

    Task<bool> TryGetAndRemoveAsync(ulong channelId, out (PendingClearAction Action, string Message) confirmation);

    void Remove(ulong channelId);
}
