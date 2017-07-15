using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Jobs
{
    abstract class Job : IDisposable
    {
        Timer _Timer;
        protected ILogger Logger { get; private set; }
        protected DateTime NextElapse { get; private set; }
        protected IGlobalConfig GlobalConfig { get; private set; }
        protected DiscordSocketClient DiscordClient { get; private set; }
        public int Interval { get; private set; }
        public Time Time { get; private set; }
        protected Job(IServiceProvider services, ILogger logger, int interval, string time) : this(services, logger, interval, ConvertToTime(time)) { }
        protected Job(IServiceProvider services, ILogger logger, int interval, Time time)
        {
            GlobalConfig = services.GetRequiredService<IGlobalConfig>();
            DiscordClient = GlobalConfig.DiscordClient;
            Interval = interval;
            Time = time;
            Logger = logger;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
        }
        async void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            Logger.Info("Timer fired.");
            await Run();
            Logger.Info("Job success.");
            StartTimerWithCloseInterval();
        }
        internal void Restart(int interval, string time)
        {
            _Timer.Stop();
            Interval = interval;
            Time = ConvertToTime(time);
            StartTimerWithCloseInterval();
        }
        internal void StartTimerWithCloseInterval()
        {
            var now = DateTime.Now;
            NextElapse = DetermineNextElapse(now);
            var interval = (NextElapse - now).TotalMilliseconds;
            _Timer.Interval = interval;
            _Timer.Start();
            TimerStartedLog();
        }
        DateTime DetermineNextElapse(DateTime now)
        {
            int difference;
            switch (Time)
            {
                case Time.Hours:
                case Time.Hour:
                    difference = DetermineDifference(now.Hour);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(difference);
                case Time.Minutes:
                case Time.Minute:
                    difference = DetermineDifference(now.Minute);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(difference);
                case Time.Seconds:
                case Time.Second:
                    difference = DetermineDifference(now.Second);
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddSeconds(difference);
                default:
                    throw new InvalidTimeException(Time);
            }
        }
        internal static Time ConvertToTime(string s) => (Time)Enum.Parse(typeof(Time), s);
        int DetermineDifference(int component) => Interval - component % Interval;
        protected virtual void TimerStartedLog() => Logger.Debug($"Config: {Interval} {Time}. Next elapse: {NextElapse}.");
        protected abstract Task Run();
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
