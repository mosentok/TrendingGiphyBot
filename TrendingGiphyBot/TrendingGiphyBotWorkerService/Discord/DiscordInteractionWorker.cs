using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordInteractionWorker(
    ILoggerWrapper<DiscordInteractionWorker> _loggerWrapper,
    IDiscordSocketClientHandler _discordSocketClientHandler,
	IDiscordSocketClientWrapper _discordSocketClient,
    InteractionService _interactionService,
	DiscordWorkerConfig _discordWorkerConfig
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			_discordSocketClient.ButtonExecuted += _discordSocketClientHandler.OnComponentExecuted;
			_discordSocketClient.InteractionCreated += _discordSocketClientHandler.OnInteractionCreated;
			_discordSocketClient.JoinedGuild += _discordSocketClientHandler.OnJoinedGuild;
			_discordSocketClient.LeftGuild += _discordSocketClientHandler.OnLeftGuild;
			_discordSocketClient.Log += _discordSocketClientHandler.OnLog;
			_discordSocketClient.ModalSubmitted += _discordSocketClientHandler.OnModalSubmitted;
			_discordSocketClient.Ready += _discordSocketClientHandler.OnReady;
			_discordSocketClient.SelectMenuExecuted += _discordSocketClientHandler.OnComponentExecuted;

			_interactionService.Log += _discordSocketClientHandler.OnLog;

			await _discordSocketClient.LoginAsync(TokenType.Bot, _discordWorkerConfig.DiscordToken);
			await _discordSocketClient.StartAsync();

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
