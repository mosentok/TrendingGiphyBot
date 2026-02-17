namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

public interface IPendingConfirmationManager
{
    Task SetPendingAsync(ulong channelId, PendingClearAction action, string message);

    bool TryRemove(ulong channelId, out PendingClearAction action);

    void Remove(ulong channelId);
}
