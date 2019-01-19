﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Exceptions;
using TrendingGiphyBot.Extensions;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot
{
    class Bot : IDisposable
    {
        readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        CommandService _Commands;
        IServiceProvider _Services;
        IGlobalConfig _GlobalConfig;
        DiscordSocketClient DiscordClient => _GlobalConfig.DiscordClient;
        List<string> _ModuleNames;
        static readonly HttpClient _HttpClient = new HttpClient();
        IFunctionHelper _FunctionHelper;
        internal async Task Run()
        {
            _FunctionHelper = new FunctionHelper();
            _Services = new ServiceCollection()
                .AddSingleton<IGlobalConfig, GlobalConfig>()
                .AddSingleton<ITrendHelper, TrendHelper>()
                .AddSingleton(_FunctionHelper)
                .BuildServiceProvider();
            _GlobalConfig = _Services.GetRequiredService<IGlobalConfig>();
            await _GlobalConfig.Initialize(_FunctionHelper);
            DiscordClient.Log += Log;
            DiscordClient.Ready += Ready;
            await DiscordClient.LoginAsync(TokenType.Bot, _GlobalConfig.Config.DiscordToken);
            await DiscordClient.StartAsync();
        }
        async Task Ready()
        {
            _Commands = new CommandService();
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());
            DetermineModuleNames();
            DiscordClient.MessageReceived += MessageReceived;
            _GlobalConfig.JobManager.Ready();
            await DiscordClient.SetGameAsync(_GlobalConfig.Config.PlayingGame);
            DiscordClient.JoinedGuild += JoinedGuild;
            DiscordClient.LeftGuild += LeftGuild;
            DiscordClient.Ready -= Ready;
        }
        void DetermineModuleNames()
        {
            var moduleNames = _Commands.Modules.Select(s => s.Name);
            var aliases = _Commands.Modules.SelectMany(s => s.Aliases);
            _ModuleNames = moduleNames.Concat(aliases).Distinct().ToList();
        }
        async Task JoinedGuild(SocketGuild arg)
        {
            await _FunctionHelper.PostStatsAsync(DiscordClient.CurrentUser.Id, DiscordClient.Guilds.Count);
        }
        async Task LeftGuild(SocketGuild arg)
        {
            await RemoveThisGuildsJobConfigs(arg);
            await _FunctionHelper.PostStatsAsync(DiscordClient.CurrentUser.Id, DiscordClient.Guilds.Count);
        }
        async Task RemoveThisGuildsJobConfigs(SocketGuild arg)
        {
            await _Logger.SwallowAsync(async () =>
            {
                var textChannelIds = arg.TextChannels.Select(s => Convert.ToDecimal(s.Id));
                foreach (var id in textChannelIds)
                    await _FunctionHelper.DeleteJobConfigAsync(id);
            });
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
                            message.HasMentionPrefix(DiscordClient.CurrentUser, ref argPos))
                        {
                            var context = new CommandContext(DiscordClient, message);
                            var result = await _Commands.ExecuteAsync(context, argPos, _Services);
                            if (!result.IsSuccess)
                                await HandleError(context, result);
                        }
                    }
                });
        }
        async Task<string> DeterminePrefix(decimal channelId)
        {
            var prefix = await _FunctionHelper.GetPrefixAsync(channelId);
            if (!string.IsNullOrEmpty(prefix))
                return prefix;
            return _GlobalConfig.Config.DefaultPrefix;
        }
        async Task HandleError(ICommandContext context, IResult result)
        {
            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand &&
                result.Error.Value != CommandError.BadArgCount)
            {
                if (result is ExecuteResult executeResult)
                    _Logger.Error(executeResult.Exception);
                var errorResult = DetermineErrorResult(result);
                var embedBuilder = await BuildErrorEmbedBuilder(context, errorResult);
                var message = string.Empty;
                try
                {
                    await context.Channel.SendMessageAsync(message, embed: embedBuilder);
                }
                catch (HttpException httpException) when (_GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
                {
                    _Logger.Warn(httpException.Message);
                    await _GlobalConfig.MessageHelper.SendMessageToUser(context, message, embedBuilder);
                }
            }
        }
        static ErrorResult DetermineErrorResult(IResult result)
        {
            if (result.Error.HasValue && result.Error.Value == CommandError.Exception)
                return new ErrorResult(CommandError.Exception, "An unexpected error occurred.", false);
            return new ErrorResult(result);
        }
        static async Task<EmbedBuilder> BuildErrorEmbedBuilder(ICommandContext context, ErrorResult errorResult)
        {
            var avatarUrl = (await context.Client.GetGuildAsync(context.Guild.Id)).IconUrl;
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(JobConfig))
                .WithIconUrl(avatarUrl);
            return new EmbedBuilder()
                .WithAuthor(author)
                .AddInlineField(nameof(errorResult.Error), errorResult.Error)
                .AddInlineField(nameof(errorResult.ErrorReason), errorResult.ErrorReason)
                .AddInlineField(nameof(errorResult.IsSuccess), errorResult.IsSuccess);
        }
        Task Log(LogMessage logMessage)
        {
            var message = logMessage.ToString(prependTimestamp: false, padSource: 0);
            switch (logMessage.Severity)
            {
                case LogSeverity.Debug:
                    _Logger.Debug(message);
                    break;
                case LogSeverity.Error:
                    _Logger.Error(message);
                    break;
                case LogSeverity.Critical:
                    _Logger.Fatal(message);
                    break;
                case LogSeverity.Verbose:
                    _Logger.Trace(message);
                    break;
                case LogSeverity.Warning:
                    _Logger.Warn(message);
                    break;
                case LogSeverity.Info:
                    _Logger.Info(message);
                    break;
                default:
                    throw new UnexpectedLogSeverityException(logMessage.Severity);
            }
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _GlobalConfig?.Dispose();
            _HttpClient?.Dispose();
        }
    }
}
