using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected ILogger Logger { get; }
        protected IGlobalConfig GlobalConfig { get; }
        protected DiscordSocketClient DiscordClient { get; }
        public int Interval { get; private set; }
        public Time Time { get; private set; }
        protected DateTime NextElapse { get; private set; }
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
        protected internal abstract Task Run();
        internal static JobConfigState DetermineJobConfigState(int interval, Time time, Config config)
        {
            var minSeconds = DetermineConfiggedSeconds(config.MinJobConfig.Interval, config.MinJobConfig.Time);
            var maxSeconds = DetermineConfiggedSeconds(config.MaxJobConfig.Interval, config.MaxJobConfig.Time);
            var configgedSeconds = DetermineConfiggedSeconds(interval, time);
            if (configgedSeconds >= minSeconds)
            {
                if (configgedSeconds <= maxSeconds)
                    switch (time)
                    {
                        case Time.Hour:
                        case Time.Hours:
                            if (config.ValidHours.Any())
                            {
                                if (config.ValidHours.Contains(interval))
                                    return JobConfigState.Valid;
                                return JobConfigState.InvalidHours;
                            }
                            return JobConfigState.InvalidTime;
                        case Time.Minute:
                        case Time.Minutes:
                            if (config.ValidMinutes.Any())
                                return IsValid(interval, JobConfigState.InvalidMinutes, config.ValidMinutes);
                            return JobConfigState.InvalidMinutes;
                        case Time.Second:
                        case Time.Seconds:
                            if (config.ValidSeconds.Any())
                                return IsValid(interval, JobConfigState.InvalidSeconds, config.ValidSeconds);
                            return JobConfigState.InvalidTime;
                        default:
                            return JobConfigState.InvalidTime;
                    }
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        static JobConfigState IsValid(int interval, JobConfigState invalidState, List<int> validMinutes)
        {
            var isValidMinuteSecond = validMinutes.Contains(interval);
            if (isValidMinuteSecond)
                return JobConfigState.Valid;
            return invalidState;
        }
        internal static double DetermineConfiggedSeconds(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval).TotalSeconds;
                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).TotalSeconds;
                case Time.Second:
                case Time.Seconds:
                    return TimeSpan.FromSeconds(interval).TotalSeconds;
                default:
                    throw new InvalidTimeException(time);
            }
        }
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
