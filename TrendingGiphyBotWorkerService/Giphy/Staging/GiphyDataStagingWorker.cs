using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GiphyDataStagingWorker
(
    ILogger<GiphyDataStagingWorker> _logger,
    IGiphyDataStage _giphyDataStage,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Giphy.Staging.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogGifStageIsRefreshing();

                await _giphyDataStage.RefreshAsync(stoppingToken);
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
}
