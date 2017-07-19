using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Enums;
using NLog;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        CommandService _Commands;
        IServiceProvider _Services;
        IGlobalConfig _GlobalConfig;
        DiscordSocketClient _DiscordClient => _GlobalConfig.DiscordClient;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        async Task MainAsync()
        {
            try
            {
                _Services = new ServiceCollection()
                    .AddSingleton<IGlobalConfig, GlobalConfig>()
                    .BuildServiceProvider();
                _GlobalConfig = _Services.GetRequiredService<IGlobalConfig>();
                _Commands = new CommandService();
                _DiscordClient.MessageReceived += MessageReceived;
                _DiscordClient.Log += Log;
                _DiscordClient.Ready += Ready;
                await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());
                await _DiscordClient.LoginAsync(TokenType.Bot, _GlobalConfig.Config.DiscordToken);
                await _DiscordClient.StartAsync();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger()?.Error(ex);
            }
        }
        async Task Ready()
        {
            var postImageJobs = new List<PostImageJob>();
            var channelsThatExist = await GetConfigsWithAliveChannels();
            AddJobs(postImageJobs, channelsThatExist, Time.Hour, Time.Hours);
            AddJobs(postImageJobs, channelsThatExist, Time.Minute, Time.Minutes);
            AddJobs(postImageJobs, channelsThatExist, Time.Second, Time.Seconds);
            _GlobalConfig.Jobs.AddRange(postImageJobs);
            _GlobalConfig.Jobs.Add(new RefreshImagesJob(_Services, _GlobalConfig.Config.RefreshImageJobConfig.Interval, _GlobalConfig.Config.RefreshImageJobConfig.Time));
            _GlobalConfig.Jobs.ForEach(s => s.StartTimerWithCloseInterval());
            await _DiscordClient.SetGameAsync(string.Empty);
            await _DiscordClient.SetGameAsync(_GlobalConfig.Config.PlayingGame);
        }
        void AddJobs(List<PostImageJob> postImageJobs, IEnumerable<JobConfig> channelsThatExist, params Time[] times)
        {
            var configs = channelsThatExist.Where(s =>
            {
                var convertedTime = s.Time.ToTime();
                return times.Contains(convertedTime);
            });
            foreach (var config in configs)
            {
                var match = postImageJobs.SingleOrDefault(s => s.Interval == config.Interval && s.Time == config.Time.ToTime());
                if (match == null)
                {
                    var postImageJob = new PostImageJob(_Services, config);
                    postImageJob.JobConfigs.Add(config);
                    postImageJobs.Add(postImageJob);
                }
                else
                    match.JobConfigs.Add(config);
            }
        }
        async Task<IEnumerable<JobConfig>> GetConfigsWithAliveChannels()
        {
            var configuredJobs = await _GlobalConfig.JobConfigDal.GetAll();
            var channelsNotFound = configuredJobs.Where(s => _DiscordClient.GetChannel(Convert.ToUInt64(s.ChannelId)) == null);
            return configuredJobs.Except(channelsNotFound);
        }
        async Task MessageReceived(SocketMessage messageParam)
        {
            if (messageParam is SocketUserMessage message)
            {
                int argPos = 0;
                if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos))
                {
                    var context = new CommandContext(_DiscordClient, message);
                    var result = await _Commands.ExecuteAsync(context, argPos, _Services);
                    if (!result.IsSuccess)
                        await HandleError(context, result);
                }
            }
        }
        static async Task HandleError(ICommandContext context, IResult result)
        {
            if (result.Error.Value != CommandError.UnknownCommand)
            {
                if (result is ExecuteResult executeResult)
                    _Logger.Error(executeResult.Exception);
                ErrorResult errorResult;
                if (result.Error.HasValue && result.Error.Value == CommandError.Exception)
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
        Task Log(LogMessage logMessage)
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
            }
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _GlobalConfig.Dispose();
        }
    }
}
