using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public class DiscordSocketClientHandler(
	ILogger<DiscordSocketClientHandler> _logger,
	DiscordSocketClient _discordSocketClient,
	InteractionService _interactionService,
	DiscordSocketClientHandlerConfig _discordSocketClientHandlerConfig,
	IServiceProvider _services
) : IDiscordSocketClientHandler
{
	public async Task OnJoinedGuildAsync(SocketGuild arg)
	{
		//TODO post stats to websites that track the bot's server count
	}

	public async Task OnLeftGuildAsync(SocketGuild arg)
	{
		//TODO post stats to websites that track the bot's server count
	}

	public Task OnLogAsync(LogMessage logMessage)
	{
		switch (logMessage.Severity)
		{
			case LogSeverity.Critical:
				_logger.LogDiscordMessage(LogLevel.Critical, logMessage);
				break;

			case LogSeverity.Debug:
				_logger.LogDiscordMessage(LogLevel.Debug, logMessage);
				break;

			case LogSeverity.Error:
				_logger.LogDiscordMessage(LogLevel.Error, logMessage);
				break;

			case LogSeverity.Info:
				_logger.LogDiscordMessage(LogLevel.Information, logMessage);
				break;

			case LogSeverity.Verbose:
				_logger.LogDiscordMessage(LogLevel.Trace, logMessage);
				break;

			case LogSeverity.Warning:
				_logger.LogDiscordMessage(LogLevel.Warning, logMessage);
				break;

			default:
				throw new ThisShouldBeImpossibleException();
		}

		return Task.CompletedTask;
	}

	public async Task OnReadyAsync()
	{
		await _discordSocketClient.SetGameAsync(_discordSocketClientHandlerConfig.PlayingGame);
		await _interactionService.AddModulesAsync(_discordSocketClientHandlerConfig.Assembly, _services);

		if (_discordSocketClientHandlerConfig.GuildToRegisterCommands is not null)
			await _interactionService.RegisterCommandsToGuildAsync(_discordSocketClientHandlerConfig.GuildToRegisterCommands.Value);
		else
			await _interactionService.RegisterCommandsGloballyAsync();
	}

	public async Task OnInteractionCreatedAsync(SocketInteraction socketInteraction)
	{
		if (socketInteraction.Type is not InteractionType.ApplicationCommand)
			return;

		var socketInteractionContext = new SocketInteractionContext(_discordSocketClient, socketInteraction);

		await _interactionService.ExecuteCommandAsync(socketInteractionContext, _services);
	}

	public async Task OnComponentExecutedAsync(SocketMessageComponent interaction)
	{
		var socketInteractionContext = new SocketInteractionContext<SocketMessageComponent>(_discordSocketClient, interaction);

		await _interactionService.ExecuteCommandAsync(socketInteractionContext, _services);
	}

	public async Task OnModalSubmittedAsync(SocketModal arg)
	{
		var socketInteractionContext = new SocketInteractionContext<SocketModal>(_discordSocketClient, arg);

		await _interactionService.ExecuteCommandAsync(socketInteractionContext, _services);
	}
}