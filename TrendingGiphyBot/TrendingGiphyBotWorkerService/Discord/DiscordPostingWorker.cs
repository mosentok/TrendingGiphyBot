using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker(
	ILoggerWrapper<DiscordPostingWorker> _loggerWrapper,
	IServiceScopeFactory _serviceScopeFactory,
	IGifPostStage _gifPostStage,
	IDiscordSocketClientWrapper _discordSocketClientWrapper,
	IDelayer _delayer,
	IntervalConfig _intervalConfig,
	TimeProvider _timeProvider
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
        {
            await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

            var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();

            var activeChannelIds = await DetermineActiveChannelIdsAsync();

            // TODO parallelize this loop?
            foreach (var channelId in activeChannelIds)
            {
                try
                {
                    var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                    if (channel is not IMessageChannel messageChannel)
                        throw new ThisShouldBeImpossibleException();

                    await messageChannel.SendMessageAsync($"*Trending!* {stagedChannelGifPosts[channelId].Url}");

                    _gifPostStage.Evict(channelId);
                }
                catch (Exception ex)
                {
                    _loggerWrapper.LogGifPostingException(ex);
                }
			}

			async Task<List<ulong>> DetermineActiveChannelIdsAsync()
			{
				var now = _timeProvider.GetUtcNow();
				var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);
				var validHours = _intervalConfig.Hours.Where(s => now.Hour % s == 0);

				using var scope = _serviceScopeFactory.CreateScope();

				var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

				return await trendingGiphyBotDbContext.ChannelSettings
					.Where(channelSettings =>
						stagedChannelGifPosts.Keys.Contains(channelSettings.ChannelId) &&
						((channelSettings.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
						(channelSettings.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(channelSettings.Frequency))))
					.Select(s => s.ChannelId)
					.ToListAsync(stoppingToken);
			}
		}
    }
}
