namespace TrendingGiphyBotWorkerService.Discord.Delaying;

public interface IDelayer
{
    Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken);
}
