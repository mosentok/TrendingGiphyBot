namespace TrendingGiphyBotWorkerService.Delaying;

public interface IDelayer
{
    Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken);
}
