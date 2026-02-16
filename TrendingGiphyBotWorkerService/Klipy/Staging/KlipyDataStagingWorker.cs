using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public class KlipyDataStagingWorker
(
    ILogger<KlipyDataStagingWorker> _logger,
    IKlipyDataStage _klipyDataStage,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Klipy.Staging.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogGifStageIsRefreshing();

                await _klipyDataStage.RefreshAsync(stoppingToken);
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
