using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker(
	ILoggerWrapper<DiscordPostingWorker> _loggerWrapper,
	ITrendingGiphyBotDbContext _trendingGiphyBotDbContext,
	IGifPostStage _gifPostStage,
	IDiscordSocketClientWrapper _discordSocketClientWrapper,
	IntervalConfig _intervalConfig,
	TimeProvider _timeProvider
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		while (!stoppingToken.IsCancellationRequested)
		{
			await timer.WaitForNextTickAsync(stoppingToken);

			var now = _timeProvider.GetUtcNow();

			var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);
			var validHours = _intervalConfig.Hours.Where(s => now.Hour % s == 0);

			var activeChannelIds = await _trendingGiphyBotDbContext.ChannelSettings
				.Where(s =>
					(s.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(s.Frequency)) ||
					(s.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(s.Frequency)))
				.Select(s => s.ChannelId)
				.ToListAsync(stoppingToken);

			// TODO parallelize this loop?
			foreach (var channelId in activeChannelIds)
			{
				try
				{
					var hasStagedGiphyData = _gifPostStage.HasStagedGiphyData(channelId);

					if (!hasStagedGiphyData)
					{
						_loggerWrapper.LogThatChannelIsNotStaged(channelId);

						continue;
					}

					var giphyData = _gifPostStage.GetStagedGiphyData(channelId);

					var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

					if (channel is not IMessageChannel messageChannel)
						throw new ThisShouldBeImpossibleException();

					await messageChannel.SendMessageAsync($"*Trending!* {giphyData.Url}");

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
