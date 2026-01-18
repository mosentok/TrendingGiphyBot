using TrendingGiphyBotWorkerService.Giphy;

namespace TrendingGiphyBotWorkerService.Discord;

public class GifStagingWorker(IGifPostStage _gifPostStage, GifStagingWorkerConfig _gifStagingWorkerConfig) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _gifPostStage.RefreshAsync();

            await Task.Delay(_gifStagingWorkerConfig.TimeSpanBetweenStageRefreshes, stoppingToken);
        }
    }
}
