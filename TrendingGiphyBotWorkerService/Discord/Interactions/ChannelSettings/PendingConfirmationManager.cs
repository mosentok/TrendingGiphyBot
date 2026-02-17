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

    public Task<bool> TryGetAndRemoveAsync(ulong channelId, out (PendingClearAction Action, string Message) confirmation)
    {
        var found = _pendingConfirmations.TryGetValue(channelId, out confirmation);

        if (found)
            _pendingConfirmations.Remove(channelId);

        return Task.FromResult(found);
    }

    public void Remove(ulong channelId) =>
        _pendingConfirmations.Remove(channelId);
}
