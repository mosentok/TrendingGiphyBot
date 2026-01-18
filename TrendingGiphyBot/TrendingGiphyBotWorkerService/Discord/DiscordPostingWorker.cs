using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker(
	ILoggerWrapper<DiscordPostingWorker> _loggerWrapper,
	IServiceScopeFactory _serviceScopeFactory,
	IGifPostStage _gifPostStage,
	IDiscordSocketClientWrapper _discordSocketClientWrapper,
	IntervalConfig _intervalConfig,
	TimeProvider _timeProvider
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
        var oneSecond = TimeSpan.FromSeconds(1);

        using var periodicTimer = new PeriodicTimer(oneSecond);

		while (!stoppingToken.IsCancellationRequested)
		{
			await periodicTimer.WaitForNextTickAsync(stoppingToken);

			var now = _timeProvider.GetUtcNow();

			// the bot should only post on minutes divisible by 5
			if (now.Minute % 5 != 0)
				continue;

			var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();

			var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);
			var validHours = _intervalConfig.Hours.Where(s => now.Hour % s == 0);

			using var scope = _serviceScopeFactory.CreateScope();

			var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

			var activeChannelIds = await trendingGiphyBotDbContext.ChannelSettings
				.Where(s =>
					stagedChannelGifPosts.Keys.Contains(s.ChannelId) &&
					((s.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(s.Frequency)) ||
					(s.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(s.Frequency))))
				.Select(s => s.ChannelId)
				.ToListAsync(stoppingToken);

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
		}
	}
}
