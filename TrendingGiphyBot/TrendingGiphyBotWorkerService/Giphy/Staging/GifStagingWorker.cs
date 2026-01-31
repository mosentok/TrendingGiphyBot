using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifStagingWorker(ILogger<GifStagingWorker> _logger, IGifPostStage _gifPostStage, GifStagingWorkerConfig _gifStagingWorkerConfig) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_gifStagingWorkerConfig.TimeSpanBetweenStageRefreshes, stoppingToken);

                _logger.LogGifStageIsRefreshing();

                await _gifPostStage.RefreshAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogTopLevelException(ex);
            }
            finally
            {
                _logger.LogGifStageHasRefreshed();
            }
    }
}
