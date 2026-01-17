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
			_discordSocketClient.ButtonExecuted += _discordSocketClientHandler.OnComponentExecutedAsync;
			_discordSocketClient.InteractionCreated += _discordSocketClientHandler.OnInteractionCreatedAsync;
			_discordSocketClient.JoinedGuild += _discordSocketClientHandler.OnJoinedGuildAsync;
			_discordSocketClient.LeftGuild += _discordSocketClientHandler.OnLeftGuildAsync;
			_discordSocketClient.Log += _discordSocketClientHandler.OnLogAsync;
			_discordSocketClient.ModalSubmitted += _discordSocketClientHandler.OnModalSubmittedAsync;
			_discordSocketClient.Ready += _discordSocketClientHandler.OnReadyAsync;
			_discordSocketClient.SelectMenuExecuted += _discordSocketClientHandler.OnComponentExecutedAsync;

			_interactionService.Log += _discordSocketClientHandler.OnLogAsync;

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
