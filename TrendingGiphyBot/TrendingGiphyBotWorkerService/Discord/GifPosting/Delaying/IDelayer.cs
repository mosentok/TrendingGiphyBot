namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Delaying;

public interface IDelayer
{
    Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken);
}
