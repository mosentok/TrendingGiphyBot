using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Random;

public class GiphyRandomCacheWorker(
    ILogger<GiphyRandomCacheWorker> _logger,
    IGiphyRandomCache _giphyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Giphy.Staging.RandomCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogGiphyRandomCacheIsRefreshing();

                await _giphyRandomCache.RefreshRandomGifsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogGiphyRandomCacheHasRefreshed();
            }
    }
}
