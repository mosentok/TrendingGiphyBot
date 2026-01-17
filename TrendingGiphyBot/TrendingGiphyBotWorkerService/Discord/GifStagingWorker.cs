namespace TrendingGiphyBotWorkerService.Discord;

public class GifStagingWorker(IGifPostStage _gifPostStage) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            await timer.WaitForNextTickAsync(stoppingToken);

            _gifPostStage.Refresh();
        }
    }
}
