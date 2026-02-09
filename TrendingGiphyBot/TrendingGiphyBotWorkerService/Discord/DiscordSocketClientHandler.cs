using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

[RegisterSingleton]
public class DiscordSocketClientHandler(
	ILogger<DiscordSocketClientHandler> _logger,
	DiscordSocketClient _discordSocketClient,
	InteractionService _interactionService,
	IOptionsMonitor<AppConfig> _appConfig,
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
		await _discordSocketClient.SetGameAsync(_appConfig.CurrentValue.Discord.SocketClientHandler.PlayingGame);

        var type = GetType();

        await _interactionService.AddModulesAsync(type.Assembly, _services);

		if (_appConfig.CurrentValue.Discord.SocketClientHandler.GuildToRegisterCommands is { } guildToRegisterCommands)
			await _interactionService.RegisterCommandsToGuildAsync(guildToRegisterCommands);
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

	public async Task OnSocketInteractionAsync<T>(T interaction) where T : SocketInteraction
    {
        var socketInteractionContext = new SocketInteractionContext<T>(_discordSocketClient, interaction);

        await _interactionService.ExecuteCommandAsync(socketInteractionContext, _services);
    }
}