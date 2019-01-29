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

namespace TrendingGiphyBotCore
{
    public class Bot : IDisposable
    {
        ILogger _Logger;
        IConfiguration _Config;
        CommandService _Commands;
        IServiceProvider _Services;
        DiscordSocketClient _DiscordClient;
        List<string> _ModuleNames;
        IFunctionHelper _FunctionHelper;
        TaskCompletionSource<bool> _LoggedInSource;
        TaskCompletionSource<bool> _ReadySource;
        public async Task Run()
        {
            _Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            _FunctionHelper = new FunctionHelper(_Config);
            _DiscordClient = new DiscordSocketClient();
            _Services = new ServiceCollection()
                .AddSingleton(_DiscordClient)
                .AddSingleton<ITrendHelper, TrendHelper>()
                .AddSingleton(_FunctionHelper)
                .AddSingleton(_Config)
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
            await _DiscordClient.SetGameAsync(_Config["PlayingGame"]);
        }
        async Task LogInAsync()
        {
            _LoggedInSource = new TaskCompletionSource<bool>();
            _DiscordClient.LoggedIn += LoggedIn;
            var token = _Config["DiscordToken"];
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
            _ModuleNames = moduleNames.Concat(aliases).Distinct().ToList();
        }
        async Task JoinedGuild(SocketGuild arg)
        {
            await _Logger.SwallowAsync(_FunctionHelper.PostStatsAsync(_DiscordClient.CurrentUser.Id, _DiscordClient.Guilds.Count));
        }
        async Task LeftGuild(SocketGuild arg)
        {
            await _Logger.SwallowAsync(async () =>
            {
                await RemoveThisGuildsJobConfigs(arg);
                await _FunctionHelper.PostStatsAsync(_DiscordClient.CurrentUser.Id, _DiscordClient.Guilds.Count);
            });
        }
        async Task RemoveThisGuildsJobConfigs(SocketGuild arg)
        {
            var textChannelIds = arg.TextChannels.Select(s => Convert.ToDecimal(s.Id));
            foreach (var id in textChannelIds)
                await _FunctionHelper.DeleteJobConfigAsync(id);
        }
        async Task MessageReceived(SocketMessage messageParam)
        {
            var isDmChannel = messageParam.Channel is IDMChannel;
            if (!isDmChannel)
                await _Logger.SwallowAsync(async () =>
                {
                    if (messageParam.IsRecognizedModule(_ModuleNames) &&
                        !messageParam.Author.IsBot &&
                        messageParam is IUserMessage message)
                    {
                        var prefix = await DeterminePrefix(messageParam.Channel.Id);
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
        async Task<string> DeterminePrefix(decimal channelId)
        {
            var prefix = await _FunctionHelper.GetPrefixAsync(channelId);
            if (!string.IsNullOrEmpty(prefix))
                return prefix;
            return _Config["DefaultPrefix"];
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
