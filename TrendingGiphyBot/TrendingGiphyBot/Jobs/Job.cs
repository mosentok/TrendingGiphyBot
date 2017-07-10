using Discord.WebSocket;
using GiphyDotNet.Manager;
using System;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Jobs
{
    abstract class Job : IDisposable
    {
        protected Giphy GiphyClient { get; private set; }
        protected DiscordSocketClient DiscordClient { get; private set; }
        public int Interval { get; set; }
        public Time Time { get; set; }
        Timer _Timer;
        protected Job(Giphy giphyClient, DiscordSocketClient discordClient, JobConfig jobConfig) : this(giphyClient, discordClient, jobConfig.Interval, jobConfig.Time) { }
        protected Job(Giphy giphyClient, DiscordSocketClient discordClient, int interval, string time)
        {
            GiphyClient = giphyClient;
            DiscordClient = discordClient;
            Interval = interval;
            Time = (Time)Enum.Parse(typeof(Time), time);
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
            switch (Time)
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
                    throw new InvalidTimeException(Time);
            }
        }
        int DetermineDifference(int component)
        {
            return Interval - component % Interval;
        }
        protected abstract Task Run();
        public async void Dispose()
        {
            await DiscordClient?.LogoutAsync();
            DiscordClient?.Dispose();
            _Timer?.Dispose();
        }
    }
}
