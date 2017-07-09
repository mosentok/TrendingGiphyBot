using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using Newtonsoft.Json;

namespace TrendingGiphyBot
{
    class Program : IDisposable
    {
        DiscordSocketClient _Client;
        Giphy _Giphy;
        Config _Config;
        Job _Job;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        async Task MainAsync()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            _Config = JsonConvert.DeserializeObject<Config>(contents);
            _Client = new DiscordSocketClient();
            _Client.Log += Log;
            _Client.Ready += Ready;
            await _Client.LoginAsync(TokenType.Bot, _Config.DiscordToken);
            await _Client.StartAsync();
            await Task.Delay(-1);
        }
        Task Ready()
        {
            _Giphy = new Giphy(_Config.GiphyToken);
            _Job = new Job(_Config.JobConfig);
            _Job.WorkToDo += Run;
            return Task.CompletedTask;
        }
        async Task Run()
        {
            var fireTime = DateTime.Now;
            Console.WriteLine($"{nameof(fireTime)}:{fireTime.ToString("o")}");
            //var gifResult = await _Giphy.TrendingGifs(new TrendingParameter { Limit = 1 });
            //var url = gifResult.Data.FirstOrDefault()?.Url;
            //if (!string.IsNullOrEmpty(url))
            //    foreach (var textChannel in _Client.Guilds.SelectMany(s => s.TextChannels))
            //    {
            //        var restUserMessage = await textChannel.SendMessageAsync(url);
            //    }
        }
        Task Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
            return Task.CompletedTask;
        }
        public async void Dispose()
        {
            await _Client?.LogoutAsync();
            _Client?.Dispose();
            _Job?.Dispose();
        }
    }
}
