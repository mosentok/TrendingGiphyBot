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

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        DiscordSocketClient _DiscordClient;
        Giphy _GiphyClient;
        Config _Config;
        List<Job> _Jobs;
        CommandService _Commands;
        IServiceProvider _Services;
        JobConfigDal _ChannelJobConfigDal;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        async Task MainAsync()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            _Config = JsonConvert.DeserializeObject<Config>(contents);
            _ChannelJobConfigDal = new JobConfigDal(_Config.ConnectionString);
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
            _Jobs = (await _ChannelJobConfigDal.GetAll()).Select(s => new PostImageJob(_GiphyClient, _DiscordClient, s)).ToList<Job>();
            //TODO base ctor only accepts string... just to convert back into Time enum
            _Jobs.Add(new RefreshImagesJob(_GiphyClient, _DiscordClient, 1, Time.Minutes.ToString()));
        }
        public async Task MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message != null)
            {
                int argPos = 0;
                if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos))
                {
                    var context = new JobConfigCommandContext(_DiscordClient, message, _GiphyClient, _Jobs, _ChannelJobConfigDal, _Config.MinimumMinutes);
                    var result = await _Commands.ExecuteAsync(context, argPos, _Services);
                    if (!result.IsSuccess)
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
        Task Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
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
