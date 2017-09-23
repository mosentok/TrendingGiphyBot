﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TrendingGiphyBot.Dals;
using NLog;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Exceptions;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        static CommandService _Commands;
        static IServiceProvider _Services;
        static IGlobalConfig _GlobalConfig;
        static DiscordSocketClient DiscordClient => _GlobalConfig.DiscordClient;
        static List<string> _ModuleNames;
        static async Task Main()
        {
            await _Logger.SwallowAsync(async () =>
            {
                _Services = new ServiceCollection()
                    .AddSingleton<IGlobalConfig, GlobalConfig>()
                    .BuildServiceProvider();
                _GlobalConfig = _Services.GetRequiredService<IGlobalConfig>();
                DiscordClient.Log += Log;
                DiscordClient.Ready += Ready;
                await DiscordClient.LoginAsync(TokenType.Bot, _GlobalConfig.Config.DiscordToken);
                await DiscordClient.StartAsync();
                await Task.Delay(-1);
            });
        }
        static async Task Ready()
        {
            _Commands = new CommandService();
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());
            DetermineModuleNames();
            DiscordClient.MessageReceived += MessageReceived;
            _GlobalConfig.JobManager.Ready();
            await DiscordClient.SetGameAsync(_GlobalConfig.Config.PlayingGame);
            await ReportStats();
            DiscordClient.JoinedGuild += JoinedGuild;
            DiscordClient.LeftGuild += LeftGuild;
            DiscordClient.Ready -= Ready;
        }
        static void DetermineModuleNames()
        {
            var moduleNames = _Commands.Modules.Select(s => s.Name);
            var aliases = _Commands.Modules.SelectMany(s => s.Aliases);
            _ModuleNames = moduleNames.Concat(aliases).Distinct().ToList();
        }
        static async Task JoinedGuild(SocketGuild arg)
        {
            await RemoveThisGuildsJobConfigs(arg);
            var jobConfig = new JobConfig
            {
                ChannelId = arg.DefaultChannel.Id,
                Interval = _GlobalConfig.Config.DefaultJobConfig.Interval,
                Time = _GlobalConfig.Config.DefaultJobConfig.Time.ToString()
            };
            await _GlobalConfig.JobConfigDal.Insert(jobConfig);
            await ReportStats();
            await arg.DefaultChannel.SendMessageAsync(string.Empty, embed: _GlobalConfig.WelcomeMessagEmbed.Value);
        }
        static async Task LeftGuild(SocketGuild arg)
        {
            await RemoveThisGuildsJobConfigs(arg);
        }
        static async Task RemoveThisGuildsJobConfigs(SocketGuild arg)
        {
            var toRemove = await arg.TextChannels.Select(s => s.Id).Where(async s => await _GlobalConfig.JobConfigDal.Any(s));
            foreach (var id in toRemove)
                await _GlobalConfig.JobConfigDal.Remove(id);
            if (await _GlobalConfig.JobConfigDal.Any(arg.Id))
                await _GlobalConfig.JobConfigDal.Remove(arg.Id);
            if (arg.DefaultChannel != null && await _GlobalConfig.JobConfigDal.Any(arg.DefaultChannel.Id))
                await _GlobalConfig.JobConfigDal.Remove(arg.DefaultChannel.Id);
        }
        static async Task ReportStats()
        {
            var content = $"{{\"server_count\":{_GlobalConfig.DiscordClient.Guilds.Count}}}";
            await ReportStats(content, $"https://discordbots.org/api/bots/{DiscordClient.CurrentUser.Id}/stats", _GlobalConfig.Config.DiscordBotsOrgToken);
            await ReportStats(content, $"https://bots.discord.pw/api/bots/{DiscordClient.CurrentUser.Id}/stats", _GlobalConfig.Config.DiscordBotsPwToken);
        }
        static async Task ReportStats(string content, string requestUri, string token)
        {
            await _Logger.SwallowAsync(async () =>
            {
                using (var httpClient = new HttpClient())
                using (var stringContent = new StringContent(content, Encoding.UTF8, "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
                    await httpClient.PostAsync(requestUri, stringContent);
                }
            });
        }
        static async Task MessageReceived(SocketMessage messageParam)
        {
            await _Logger.SwallowAsync(async () =>
            {
                if (messageParam.Channel is SocketTextChannel &&
                    messageParam is SocketUserMessage message &&
                    !message.Author.IsBot &&
                    message.IsRecognizedModule(_ModuleNames))
                {
                    var prefix = await DeterminePrefix(message);
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
        static async Task<string> DeterminePrefix(SocketUserMessage message)
        {
            if (await _GlobalConfig.ChannelConfigDal.Any(message.Channel.Id))
                return await _GlobalConfig.ChannelConfigDal.GetPrefix(message.Channel.Id);
            return _GlobalConfig.Config.DefaultPrefix;
        }
        static async Task HandleError(ICommandContext context, IResult result)
        {
            if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
            {
                if (result is ExecuteResult executeResult)
                    _Logger.Error(executeResult.Exception);
                ErrorResult errorResult;
                if (result.Error.Value == CommandError.Exception)
                    errorResult = new ErrorResult(CommandError.Exception, "An unexpected error occurred.", false);
                else
                    errorResult = new ErrorResult(result);
                var avatarUrl = (await context.Client.GetGuildAsync(context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName(nameof(JobConfig))
                    .WithIconUrl(avatarUrl);
                var embed = new EmbedBuilder()
                    .WithAuthor(author)
                    .AddInlineField(nameof(errorResult.Error), errorResult.Error)
                    .AddInlineField(nameof(errorResult.ErrorReason), errorResult.ErrorReason)
                    .AddInlineField(nameof(errorResult.IsSuccess), errorResult.IsSuccess);
                await context.Channel.SendMessageAsync(string.Empty, embed: embed);
            }
        }
        static Task Log(LogMessage logMessage)
        {
            var message = logMessage.ToString();
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
        }
    }
}
