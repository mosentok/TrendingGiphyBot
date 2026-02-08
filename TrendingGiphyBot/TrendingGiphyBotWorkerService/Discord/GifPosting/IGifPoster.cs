namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public interface IGifPoster
{
    Task PostGifsAsync(CancellationToken stoppingToken);
}
