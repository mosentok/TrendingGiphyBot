using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Delaying;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public class DiscordPostingWorker
(
    ILogger<DiscordPostingWorker> _logger,
    IDelayer _delayer,
    IGifPoster _gifPoster
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

                _logger.LogPostingGifs();

                await _gifPoster.PostGifsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogTopLevelException(ex);
            }
            finally
            {
                _logger.LogPostedGifs();
            }
    }
}
