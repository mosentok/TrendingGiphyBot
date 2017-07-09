using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        DiscordSocketClient _DiscordClient;
        Giphy _GiphyClient;
        Config _Config;
        Job _Job;
        CommandService _Commands;
        IServiceProvider _Services;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        async Task MainAsync()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            _Config = JsonConvert.DeserializeObject<Config>(contents);
            if (_Config.JobConfig.IsValid)
            {
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
            else
                throw new InvalidOperationException(_Config.JobConfig.MinMinutesMessage);
        }
        Task Ready()
        {
            _GiphyClient = new Giphy(_Config.GiphyToken);
            _Job = new Job(_Config.JobConfig);
            _Job.WorkToDo += Run;
            return Task.CompletedTask;
        }
        public async Task MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message != null)
            {
                int argPos = 0;
                if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos))) return;
                var context = new JobConfigCommandContext(_DiscordClient, message, _Job);
                var result = await _Commands.ExecuteAsync(context, argPos, _Services);
                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
        async Task Run()
        {
            var fireTime = DateTime.Now;
            Console.WriteLine($"{nameof(fireTime)}:{fireTime.ToString("o")}");
            var gifResult = await _GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            if (!string.IsNullOrEmpty(url))
                foreach (var textChannel in _DiscordClient.Guilds.SelectMany(s => s.TextChannels))
                {
                    var restUserMessage = await textChannel.SendMessageAsync(url);
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
            _Job?.Dispose();
        }
    }
}
