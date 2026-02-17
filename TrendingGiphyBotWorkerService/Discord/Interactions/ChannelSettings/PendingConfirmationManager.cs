namespace TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

[RegisterSingleton]
public class PendingConfirmationManager : IPendingConfirmationManager
{
    private readonly Dictionary<ulong, (PendingClearAction Action, string Message)> _pendingConfirmations = new();

    public Task SetPendingAsync(ulong channelId, PendingClearAction action, string message)
    {
        _pendingConfirmations[channelId] = (action, message);

        return Task.CompletedTask;
    }

    public bool TryRemove(ulong channelId, out PendingClearAction action)
    {
        var found = _pendingConfirmations.TryGetValue(channelId, out var confirmation);

        action = confirmation.Action;

        if (found)
            _pendingConfirmations.Remove(channelId);

        return found;
    }

    public void Remove(ulong channelId) =>
        _pendingConfirmations.Remove(channelId);
}
