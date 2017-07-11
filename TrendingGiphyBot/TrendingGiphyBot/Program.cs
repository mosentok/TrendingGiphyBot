using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using TrendingGiphyBot.CommandContexts;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Enums;
using NLog;

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        DiscordSocketClient _DiscordClient;
        Giphy _GiphyClient;
        Config _Config;
        List<Job> _Jobs;
        CommandService _Commands;
        IServiceProvider _Services;
        JobConfigDal _JobConfigDal;
        UrlCacheDal _UrlCacheDal;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        async Task MainAsync()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            _Config = JsonConvert.DeserializeObject<Config>(contents);
            _JobConfigDal = new JobConfigDal(_Config.ConnectionString);
            _UrlCacheDal = new UrlCacheDal(_Config.ConnectionString);
            _DiscordClient = new DiscordSocketClient();
            _Commands = new CommandService();
            _Services = new ServiceCollection().BuildServiceProvider();
            _DiscordClient.MessageReceived += MessageReceived;
            _DiscordClient.Log += Log;
            _DiscordClient.Ready += Ready;
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await _DiscordClient.LoginAsync(TokenType.Bot, _Config.DiscordToken);
            await _DiscordClient.StartAsync();
            await Task.Delay(-1);
        }
        async Task Ready()
        {
            _GiphyClient = new Giphy(_Config.GiphyToken);
            _Jobs = new List<Job>();
            var postImageJobs = (await _JobConfigDal.GetAll()).Select(s => new PostImageJob(_GiphyClient, _DiscordClient, s, _JobConfigDal, _UrlCacheDal));
            _Jobs.AddRange(postImageJobs);
            //TODO base ctor only accepts string... just to convert back into Time enum
            _Jobs.Add(new RefreshImagesJob(_GiphyClient, _DiscordClient, 1, Time.Minutes.ToString(), _UrlCacheDal));
            _Jobs.Add(new SetGameJob(_GiphyClient, _DiscordClient, 1, Time.Minutes.ToString(), _JobConfigDal));
            _Jobs.ForEach(s => s.StartTimerWithCloseInterval());
            var count = await _JobConfigDal.GetCount();
            await _DiscordClient.SetGameAsync(string.Empty);
            await _DiscordClient.SetGameAsync($"A Tale of {count} Gifs");
        }
        public async Task MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message != null)
            {
                int argPos = 0;
                if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos))
                {
                    var context = new JobConfigCommandContext(_DiscordClient, message, _GiphyClient, _Jobs, _JobConfigDal, _UrlCacheDal, _Config.MinimumMinutes);
                    var result = await _Commands.ExecuteAsync(context, argPos, _Services);
                    if (!result.IsSuccess)
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                }
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
        public async void Dispose()
        {
            await _DiscordClient?.LogoutAsync();
            _DiscordClient?.Dispose();
            _Jobs?.ForEach(s => s?.Dispose());
        }
    }
}
