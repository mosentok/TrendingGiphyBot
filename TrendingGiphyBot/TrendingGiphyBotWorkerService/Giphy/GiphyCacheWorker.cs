using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyCacheWorker(
    ILoggerWrapper<GiphyCacheWorker> _loggerWrapper,
    IGiphyClient _giphyClient,
    IGifCache _gifCache,
	GiphyCacheWorkerConfig _giphyCacheWorkerConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var numberOfResponses = 0;

                for (var numberOfLoops = 0; numberOfLoops < _giphyCacheWorkerConfig.MaxGiphyCacheLoops && numberOfResponses % _giphyCacheWorkerConfig.MaxPageCount == 0; numberOfLoops++)
                {
                    var giphyResponse = await _giphyClient.GetTrendingGifsAsync(offset: numberOfResponses, cancellationToken: stoppingToken);

                    _gifCache.Add(giphyResponse.Data);

                    numberOfResponses += giphyResponse.Data.Count;
                }

                await Task.Delay(_giphyCacheWorkerConfig.TimeSpanBetweenRefreshes, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _loggerWrapper.LogTopLevelException(exception);
        }
    }
}
