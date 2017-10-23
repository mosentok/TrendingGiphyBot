using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        internal async Task Run()
        {
            _Services = new ServiceCollection()
                .AddSingleton<IGlobalConfig, GlobalConfig>()
                .BuildServiceProvider();
            _GlobalConfig = _Services.GetRequiredService<IGlobalConfig>();
            await _GlobalConfig.Initialize();
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
            await PostStats();
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
            await RemoveThisGuildsJobConfigs(arg);
            if (arg.DefaultChannel != null)
                await PostToDefaultChannel(arg);
            else
                await PostToOwner(arg);
            await PostStats();
        }
        async Task PostToDefaultChannel(SocketGuild arg)
        {
            var jobConfig = new JobConfig
            {
                ChannelId = arg.DefaultChannel.Id,
                Interval = _GlobalConfig.Config.DefaultJobConfig.Interval,
                Time = _GlobalConfig.Config.DefaultJobConfig.Time.ToString()
            };
            await _GlobalConfig.JobConfigDal.Insert(jobConfig);
            var embed = _GlobalConfig.BuildEmbedFromConfig(_GlobalConfig.Config.WelcomeMessageDefault);
            if (!string.IsNullOrEmpty(_GlobalConfig.Config.WelcomeMessageDefault.FooterText))
                embed.Footer = new EmbedFooterBuilder()
                    .WithText(_GlobalConfig.Config.WelcomeMessageDefault.FooterText);
            await arg.DefaultChannel.SendMessageAsync(string.Empty, embed: embed);
        }
        async Task PostToOwner(SocketGuild arg)
        {
            var embed = _GlobalConfig.BuildEmbedFromConfig(_GlobalConfig.Config.WelcomeMessageOwner);
            if (!string.IsNullOrEmpty(_GlobalConfig.Config.WelcomeMessageOwner.FooterText))
            {
                var footerWithGuild = string.Format(_GlobalConfig.Config.WelcomeMessageOwner.FooterText, arg.Name);
                embed.Footer = new EmbedFooterBuilder()
                    .WithText(footerWithGuild);
            }
            await arg.Owner.SendMessageAsync(string.Empty, embed: embed);
        }
        async Task LeftGuild(SocketGuild arg)
        {
            await RemoveThisGuildsJobConfigs(arg);
            await PostStats();
        }
        async Task RemoveThisGuildsJobConfigs(SocketGuild arg)
        {
            var toRemove = await arg.TextChannels.Select(s => s.Id).WhereAsync(async s => await _GlobalConfig.JobConfigDal.Any(s));
            foreach (var id in toRemove)
                await _GlobalConfig.JobConfigDal.Remove(id);
            if (await _GlobalConfig.JobConfigDal.Any(arg.Id))
                await _GlobalConfig.JobConfigDal.Remove(arg.Id);
            if (arg.DefaultChannel != null && await _GlobalConfig.JobConfigDal.Any(arg.DefaultChannel.Id))
                await _GlobalConfig.JobConfigDal.Remove(arg.DefaultChannel.Id);
        }
        async Task PostStats()
        {
            if (_GlobalConfig.Config.StatPosts != null)
                foreach (var statPost in _GlobalConfig.Config.StatPosts)
                    await PostStat(statPost);
        }
        async Task PostStat(StatPost statPost)
        {
            await _Logger.SwallowAsync(async () =>
            {
                var content = $"{{\"server_count\":{_GlobalConfig.DiscordClient.Guilds.Count}}}";
                var requestUri = string.Format(statPost.UrlStringFormat, DiscordClient.CurrentUser.Id);
                using (var httpClient = new HttpClient())
                using (var stringContent = new StringContent(content, Encoding.UTF8, "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(statPost.Token);
                    await httpClient.PostAsync(requestUri, stringContent);
                }
            });
        }
        async Task MessageReceived(SocketMessage messageParam)
        {
            await _Logger.SwallowAsync(async () =>
            {
                if (messageParam.IsRecognizedModule(_ModuleNames) &&
                    !messageParam.Author.IsBot &&
                    messageParam is SocketUserMessage message)
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
        async Task<string> DeterminePrefix(SocketUserMessage message)
        {
            if (await _GlobalConfig.ChannelConfigDal.Any(message.Channel.Id))
                return await _GlobalConfig.ChannelConfigDal.GetPrefix(message.Channel.Id);
            return _GlobalConfig.Config.DefaultPrefix;
        }
        async Task HandleError(ICommandContext context, IResult result)
        {
            if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
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
        }
    }
}
