using Discord.WebSocket;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot
{
    class Job : IDisposable
    {
        readonly Giphy _GiphyClient;
        readonly DiscordSocketClient _DiscordClient;
        internal JobConfig JobConfig { get; private set; }
        Timer _Timer;
        public Job(Giphy giphyClient, DiscordSocketClient discordClient, JobConfig jobConfig)
        {
            _GiphyClient = giphyClient;
            _DiscordClient = discordClient;
            JobConfig = jobConfig;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
            StartTimerWithCloseInterval();
        }
        async void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            await Run();
            StartTimerWithCloseInterval();
        }
        void StartTimerWithCloseInterval()
        {
            var now = DateTime.Now;
            var nextElapse = DetermineNextElapse(now);
            var interval = (nextElapse - now).TotalMilliseconds;
            _Timer.Interval = interval;
            _Timer.Start();
        }
        DateTime DetermineNextElapse(DateTime now)
        {
            int difference;
            switch ((Time)Enum.Parse(typeof(Time), JobConfig.Time))
            {
                case Time.Hours:
                    difference = DetermineDifference(now.Hour);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(difference);
                case Time.Minutes:
                    difference = DetermineDifference(now.Minute);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(difference);
                case Time.Seconds:
                    difference = DetermineDifference(now.Second);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(difference);
                default:
                    throw new InvalidTimeException(JobConfig.Time);
            }
        }
        int DetermineDifference(int component)
        {
            return JobConfig.Interval - component % JobConfig.Interval;
        }
        async Task Run()
        {
            var fireTime = DateTime.Now;
            Console.WriteLine($"{nameof(fireTime)}:{fireTime.ToString("o")}");
            var gifResult = await _GiphyClient.TrendingGifs(new TrendingParameter { Limit = 1 });
            var url = gifResult.Data.FirstOrDefault()?.Url;
            if (!string.IsNullOrEmpty(url))
            {
                var channelId = Convert.ToUInt64(JobConfig.ChannelId);
                var socketTextChannel = _DiscordClient.GetChannel(channelId) as SocketTextChannel;
                await socketTextChannel?.SendMessageAsync(url);
            }
        }
        public async void Dispose()
        {
            await _DiscordClient?.LogoutAsync();
            _DiscordClient?.Dispose();
            _Timer?.Dispose();
        }
    }
}
