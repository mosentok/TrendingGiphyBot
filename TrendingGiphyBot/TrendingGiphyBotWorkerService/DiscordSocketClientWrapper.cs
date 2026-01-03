using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService;

public static class SocketUserMessageExtensions
{
	public static bool IsRecognizedModule(this SocketMessage message, List<string> modules) =>
		modules.Any(module => message.Content.Contains(module, StringComparison.CurrentCultureIgnoreCase));
}

public class DiscordSocketClientHandler(ILoggerWrapper<DiscordSocketClientHandler> _loggerWrapper, DiscordSocketClient _discordSocketClient, CommandService _commandService, List<ulong> _listenToOnlyTheseChannels)
{
	public async Task JoinedGuild(SocketGuild arg)
	{
		//TODO post stats to websites that track the bot's server count
		//await _loggerWrapper.SwallowAsync(_FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count));
	}

	public async Task LeftGuild(SocketGuild arg)
	{
		//TODO post stats to websites that track the bot's server count
		//await _loggerWrapper.SwallowAsync(async () =>
		//{
		//	await RemoveThisGuildsJobConfigs(arg);
		//	await _FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count);
		//});
	}

	public Task Log(LogMessage logMessage)
	{
		switch (logMessage.Severity)
		{
			case LogSeverity.Critical:
				_loggerWrapper.LogDiscordMessage(LogLevel.Critical, logMessage);
				break;
			case LogSeverity.Debug:
				_loggerWrapper.LogDiscordMessage(LogLevel.Debug, logMessage);
				break;
			case LogSeverity.Error:
				_loggerWrapper.LogDiscordMessage(LogLevel.Error, logMessage);
				break;
			case LogSeverity.Info:
				_loggerWrapper.LogDiscordMessage(LogLevel.Information, logMessage);
				break;
			case LogSeverity.Verbose:
				_loggerWrapper.LogDiscordMessage(LogLevel.Trace, logMessage);
				break;
			case LogSeverity.Warning:
				_loggerWrapper.LogDiscordMessage(LogLevel.Warning, logMessage);
				break;
			default:
				throw new UnexpectedLogSeverityException(logMessage.Severity);
		}

		return Task.CompletedTask;
	}

	public async Task MessageReceived(SocketMessage messageParam)
	{
		if (_listenToOnlyTheseChannels.Count > 0 && !_listenToOnlyTheseChannels.Contains(messageParam.Channel.Id))
			return;

		var isDmChannel = messageParam.Channel is IDMChannel;
		if (isDmChannel)
			return;

		await _loggerWrapper.SwallowAsync(async () =>
		{
			if (messageParam.IsRecognizedModule(_ModuleNames) &&
				!messageParam.Author.IsBot &&
				messageParam is IUserMessage message)
			{
				var prefix = DeterminePrefix(messageParam.Channel.Id);
				var argPos = 0;
				if (message.HasStringPrefix(prefix, ref argPos) ||
					message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))
				{
					var context = new CommandContext(_discordSocketClient, message);
					var result = await _commandService.ExecuteAsync(context, argPos, _Services);
					if (!result.IsSuccess &&
						result.Error.HasValue &&
						result.Error.Value != CommandError.UnknownCommand &&
						result.Error.Value != CommandError.BadArgCount &&
						result is ExecuteResult executeResult)
						_Logger.LogError(executeResult.Exception, $"Error processing message content '{message.Content}'.");
				}
				string DeterminePrefix(decimal channelId)
				{
					var found = _PrefixDictionary.TryGetValue(channelId, out var prefix);
					if (found)
						return prefix;
					return _ConfigWrapper["DefaultPrefix"];
				}
			}
		});
	}
}
public class DiscordSocketClientWrapper
{
	readonly DiscordSocketClient _discordSocketClient;
	readonly ILogger<DiscordSocketClientWrapper> _logger;
	public DiscordSocketClientWrapper(DiscordSocketClient discordSocketClient, DiscordSocketClientHandler discordSocketClientHandler, ILogger<DiscordSocketClientWrapper> logger)
	{
		_discordSocketClient = discordSocketClient;
		_logger = logger;

		_discordSocketClient.Log += discordSocketClientHandler.Log;
		_discordSocketClient.MessageReceived += discordSocketClientHandler.MessageReceived;
		_discordSocketClient.JoinedGuild += discordSocketClientHandler.JoinedGuild;
		_discordSocketClient.LeftGuild += discordSocketClientHandler.LeftGuild;
	}
}
