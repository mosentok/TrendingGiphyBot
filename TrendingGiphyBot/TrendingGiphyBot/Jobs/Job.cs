using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Jobs
{
    abstract class Job : IDisposable
    {
        ILogger _Logger;
        protected DateTime NextElapse { get; private set; }
        Timer _Timer;
        protected IGlobalConfig GlobalConfig { get; private set; }
        protected DiscordSocketClient DiscordClient { get; private set; }
        public int Interval { get; private set; }
        public Time Time { get; private set; }
        public Job(IServiceProvider services, DiscordSocketClient discordClient, JobConfig jobConfig, ILogger logger)
        {
            Initialize(services, discordClient, jobConfig.Interval, jobConfig.Time.ToString(), logger);
        }
        public Job(IServiceProvider services, DiscordSocketClient discordClient, int interval, string time, ILogger logger)
        {
            Initialize(services, discordClient, interval, time, logger);
        }
        void Initialize(IServiceProvider services, DiscordSocketClient discordClient, int interval, string time, ILogger logger)
        {
            GlobalConfig = services.GetRequiredService<IGlobalConfig>();
            DiscordClient = discordClient;
            Interval = interval;
            Time = ConvertToTime(time);
            _Logger = logger;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
        }
        static Time ConvertToTime(string s) => (Time)Enum.Parse(typeof(Time), s);
        async void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            _Logger.Info("Timer fired.");
            await Run();
            StartTimerWithCloseInterval();
        }
        protected virtual void TimerStartedLog() => _Logger.Info($"Config: {Interval} {Time}. Next elapse: {NextElapse}.");
        internal void Restart(JobConfig jobConfig)
        {
            _Timer.Stop();
            Interval = jobConfig.Interval;
            Time = ConvertToTime(jobConfig.Time);
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
        int DetermineDifference(int component)
        {
            return Interval - component % Interval;
        }
        protected abstract Task Run();
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
