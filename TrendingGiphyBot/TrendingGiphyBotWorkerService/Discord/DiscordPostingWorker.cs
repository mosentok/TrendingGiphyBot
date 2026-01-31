using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker
(
    ILogger<DiscordPostingWorker> _logger,
    IGifPostStage _gifPostStage,
    IDelayer _delayer,
    IDiscordChannelGifPoster _discordChannelGifPoster,
    IChannelSettingsFinder _channelFinder
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

                _logger.LogPostingGifs();

                var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();

                var channelIdsInPostingHours = await _channelFinder.GetChannelSettingsIdsReadyToPostAsync(stagedChannelGifPosts.Keys, stoppingToken);

                await _discordChannelGifPoster.PostGifsAsync(stagedChannelGifPosts, channelIdsInPostingHours, stoppingToken);
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
