using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifStagingWorker
(
    ILogger<GifStagingWorker> _logger,
    IGiphyDataStage _gifPostStage,
    IOptions<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.Value.Giphy.Staging.TimeSpanBetweenRefreshes, stoppingToken);

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
