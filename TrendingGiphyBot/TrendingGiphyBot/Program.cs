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
        Timer _Timer;
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
            _Timer = new Timer();
            _Timer.Elapsed += (a, b) => Elapsed();
            StartTimerWithCloseInterval();
            return Task.CompletedTask;
        }
        void StartTimerWithCloseInterval()
        {
            var jobIntervalSeconds = DetermineJobIntervalSeconds();
            var differenceSeconds = DetermineDifferenceSeconds(jobIntervalSeconds);
            var now = DateTime.Now;
            var nextElapse = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(differenceSeconds);
            var interval = (nextElapse - DateTime.Now).TotalMilliseconds;
            _Timer.Interval = interval;
            _Timer.Start();
        }
        int DetermineJobIntervalSeconds()
        {
            switch (_Config.JobConfig.Time)
            {
                case Time.Hours:
                    return (int)TimeSpan.FromHours(_Config.JobConfig.Interval).TotalSeconds;
                case Time.Minutes:
                    return (int)TimeSpan.FromMinutes(_Config.JobConfig.Interval).TotalSeconds;
                case Time.Seconds:
                    return (int)TimeSpan.FromSeconds(_Config.JobConfig.Interval).TotalSeconds;
                default:
                    throw new InvalidOperationException($"{_Config.JobConfig.Time} is an invalid {nameof(Time)}.");
            }
        }
        static int DetermineDifferenceSeconds(int runEveryXSeconds)
        {
            return runEveryXSeconds - DateTime.Now.Second % runEveryXSeconds;
        }
        async void Elapsed()
        {
            _Timer.Stop();
            var fireTime = DateTime.Now;
            Console.WriteLine($"{nameof(fireTime)}:{fireTime.ToString("o")}");
            //var gifResult = await _Giphy.TrendingGifs(new TrendingParameter { Limit = 1 });
            //var url = gifResult.Data.FirstOrDefault()?.Url;
            //if (!string.IsNullOrEmpty(url))
            //    foreach (var textChannel in _Client.Guilds.SelectMany(s => s.TextChannels))
            //    {
            //        var restUserMessage = await textChannel.SendMessageAsync(url);
            //    }
            StartTimerWithCloseInterval();
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
            _Timer?.Dispose();
        }
    }
}
