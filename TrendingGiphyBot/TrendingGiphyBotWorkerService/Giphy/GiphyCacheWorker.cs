using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyCacheWorker(
    ILoggerWrapper<GiphyCacheWorker> _loggerWrapper,
    IGiphyClient _giphyClient,
    IGifCache _gifCache,
	GiphyConfig _giphyConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var numberOfResponses = 0;

                while (numberOfResponses % _giphyConfig.MaxPageCount == 0)
                {
                    var giphyResponse = await _giphyClient.GetTrendingGifsAsync(offset: numberOfResponses, cancellationToken: stoppingToken);

                    _gifCache.Add(giphyResponse.Data);

                    numberOfResponses += giphyResponse.Data.Count;
                }

                await Task.Delay(_giphyConfig.TimeSpanBetweenRefreshes, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _loggerWrapper.LogTopLevelException(exception);
        }
    }
}
