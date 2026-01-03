using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace TrendingGiphyBotWorkerService;

public class Bot(DiscordSocketClient _discordSocketClient, CommandService _commandService, string discordToken, string playingGame)
{
	ILogger _Logger;
	IServiceProvider _Services;
	List<string> _ModuleNames;
	TaskCompletionSource<bool> _LoggedInSource;
	TaskCompletionSource<bool> _ReadySource;
	List<ulong> _ListenToOnlyTheseChannels;
	Dictionary<decimal, string> _PrefixDictionary;
	public async Task Run()
	{
		//TODO pull this from the database
		_PrefixDictionary = [];

		_discordSocketClient.Log += Log;
		await LogInAsync();
		await StartAsync();

        var entryAssembly = Assembly.GetEntryAssembly();

		ArgumentNullException.ThrowIfNull(entryAssembly);

        await _commandService.AddModulesAsync(entryAssembly, _Services);
		DetermineModuleNames();
		_discordSocketClient.MessageReceived += MessageReceived;
		_discordSocketClient.JoinedGuild += JoinedGuild;
		_discordSocketClient.LeftGuild += LeftGuild;
		await _discordSocketClient.SetGameAsync(playingGame);
	}
	void TrendHelper_PrefixUpdated(decimal channelId, string prefix)
	{
		_PrefixDictionary[channelId] = prefix;
	}
	async Task LogInAsync()
	{
		_LoggedInSource = new TaskCompletionSource<bool>();
		_discordSocketClient.LoggedIn += LoggedIn;
		await _discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
		//TODO debug and see if this task completion source is actually needed
		await _LoggedInSource.Task;
		_discordSocketClient.LoggedIn -= LoggedIn;
	}
	Task LoggedIn()
	{
		_LoggedInSource.SetResult(true);
		return Task.CompletedTask;
	}
	async Task StartAsync()
	{
		_ReadySource = new TaskCompletionSource<bool>();
		_discordSocketClient.Ready += Ready;
		await _discordSocketClient.StartAsync();
		await _ReadySource.Task;
		_discordSocketClient.Ready -= Ready;
	}
	Task Ready()
	{
		_ReadySource.SetResult(true);
		return Task.CompletedTask;
	}
	void DetermineModuleNames()
	{
		var moduleNames = _Commands.Modules.Select(s => s.Name);
		var aliases = _Commands.Modules.SelectMany(s => s.Aliases);
		_ModuleNames = moduleNames.Concat(aliases).Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();
	}
	async Task JoinedGuild(SocketGuild arg)
	{
		await _Logger.SwallowAsync(_FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count));
	}
	async Task LeftGuild(SocketGuild arg)
	{
		await _Logger.SwallowAsync(async () =>
		{
			await RemoveThisGuildsJobConfigs(arg);
			await _FunctionWrapper.PostStatsAsync(_discordSocketClient.CurrentUser.Id, _discordSocketClient.Guilds.Count);
		});
	}
	async Task RemoveThisGuildsJobConfigs(SocketGuild arg)
	{
		var textChannelIds = arg.TextChannels.Select(s => Convert.ToDecimal(s.Id));
		foreach (var id in textChannelIds)
			await _FunctionWrapper.DeleteJobConfigAsync(id);
	}
	async Task MessageReceived(SocketMessage messageParam)
	{
		if (_ListenToOnlyTheseChannels != null && !_ListenToOnlyTheseChannels.Contains(messageParam.Channel.Id))
			return;
		var isDmChannel = messageParam.Channel is IDMChannel;
		if (isDmChannel)
			return;
		await _Logger.SwallowAsync(async () =>
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
					var result = await _Commands.ExecuteAsync(context, argPos, _Services);
					if (!result.IsSuccess &&
						result.Error.HasValue &&
						result.Error.Value != CommandError.UnknownCommand &&
						result.Error.Value != CommandError.BadArgCount &&
						result is ExecuteResult executeResult)
						_Logger.LogError(executeResult.Exception, $"Error processing message content '{message.Content}'.");
				}
			}
		});
	}
	Task Log(LogMessage logMessage)
	{
		var message = logMessage.ToString(prependTimestamp: false, padSource: 0);
		switch (logMessage.Severity)
		{
			case LogSeverity.Debug:
				_Logger.LogDebug(message);
				break;
			case LogSeverity.Error:
				_Logger.LogError(message);
				break;
			case LogSeverity.Critical:
				_Logger.LogCritical(message);
				break;
			case LogSeverity.Verbose:
				_Logger.LogTrace(message);
				break;
			case LogSeverity.Warning:
				_Logger.LogWarning(message);
				break;
			case LogSeverity.Info:
				_Logger.LogInformation(message);
				break;
			default:
				throw new UnexpectedLogSeverityException(logMessage.Severity);
		}
		return Task.CompletedTask;
	}
}
