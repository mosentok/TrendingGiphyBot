using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService;

public class Worker(ILoggerWrapper<Worker> _loggerWrapper, DiscordSocketClient _discordSocketClient) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			await Task.Delay(Timeout.Infinite, stoppingToken);
		}
		catch (Exception exception)
		{
			_loggerWrapper.LogTopLevelException(exception);
		}
		finally
		{
			await _discordSocketClient.LogoutAsync();
		}
	}
}
