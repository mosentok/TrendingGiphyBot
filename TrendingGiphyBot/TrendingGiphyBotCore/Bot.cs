using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;
using TrendingGiphyBotCore.Helpers;
using TrendingGiphyBotCore.Wrappers;

namespace TrendingGiphyBotCore
{
    public class Bot : IDisposable
    {
        ILogger _Logger;
        IConfigurationWrapper _ConfigWrapper;
        CommandService _Commands;
        IServiceProvider _Services;
        DiscordSocketClient _DiscordClient;
        List<string> _ModuleNames;
        IFunctionWrapper _FunctionWrapper;
        TaskCompletionSource<bool> _LoggedInSource;
        TaskCompletionSource<bool> _ReadySource;
        List<ulong> _ListenToOnlyTheseChannels;
        Dictionary<decimal, string> _PrefixDictionary;
        public async Task Run()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            _ConfigWrapper = new ConfigurationWrapper(config);
            _ListenToOnlyTheseChannels = _ConfigWrapper.Get<List<ulong>>("ListenToOnlyTheseChannels");
            _FunctionWrapper = new FunctionWrapper(_ConfigWrapper);
            _DiscordClient = new DiscordSocketClient();
            var trendHelper = new TrendHelper(_ConfigWrapper);
            trendHelper.PrefixUpdated += TrendHelper_PrefixUpdated;
            _Services = new ServiceCollection()
                .AddSingleton(_DiscordClient)
                .AddSingleton<ITrendHelper>(trendHelper)
                .AddSingleton(_FunctionWrapper)
                .AddSingleton(_ConfigWrapper)
                .AddLogging(s => s.AddConsole())
                .BuildServiceProvider();
            _Logger = _Services.GetService<ILogger<Bot>>();
            _DiscordClient.Log += Log;
            await LogInAsync();
            await StartAsync();
            _Commands = new CommandService();
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
            DetermineModuleNames();
            _DiscordClient.MessageReceived += MessageReceived;
            _DiscordClient.JoinedGuild += JoinedGuild;
            _DiscordClient.LeftGuild += LeftGuild;
            await _DiscordClient.SetGameAsync(_ConfigWrapper["PlayingGame"]);
            _PrefixDictionary = await _FunctionWrapper.GetPrefixDictionaryAsync();
        }
        void TrendHelper_PrefixUpdated(decimal channelId, string prefix)
        {
            _PrefixDictionary[channelId] = prefix;
        }
        async Task LogInAsync()
        {
            _LoggedInSource = new TaskCompletionSource<bool>();
            _DiscordClient.LoggedIn += LoggedIn;
            var token = _ConfigWrapper["DiscordToken"];
            await _DiscordClient.LoginAsync(TokenType.Bot, token);
            await _LoggedInSource.Task;
            _DiscordClient.LoggedIn -= LoggedIn;
        }
        Task LoggedIn()
        {
            _LoggedInSource.SetResult(true);
            return Task.CompletedTask;
        }
        async Task StartAsync()
        {
            _ReadySource = new TaskCompletionSource<bool>();
            _DiscordClient.Ready += Ready;
            await _DiscordClient.StartAsync();
            await _ReadySource.Task;
            _DiscordClient.Ready -= Ready;
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
            await _Logger.SwallowAsync(_FunctionWrapper.PostStatsAsync(_DiscordClient.CurrentUser.Id, _DiscordClient.Guilds.Count));
        }
        async Task LeftGuild(SocketGuild arg)
        {
            await _Logger.SwallowAsync(async () =>
            {
                await RemoveThisGuildsJobConfigs(arg);
                await _FunctionWrapper.PostStatsAsync(_DiscordClient.CurrentUser.Id, _DiscordClient.Guilds.Count);
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
                        message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos))
                    {
                        var context = new CommandContext(_DiscordClient, message);
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
        string DeterminePrefix(decimal channelId)
        {
            var found = _PrefixDictionary.TryGetValue(channelId, out var prefix);
            if (found)
                return prefix;
            return _ConfigWrapper["DefaultPrefix"];
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
        public void Dispose()
        {
            //TODO use task completion source?
            _DiscordClient?.LogoutAsync()?.Wait();
            _DiscordClient?.Dispose();
        }
    }
}
