using Discord.WebSocket;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;

namespace TrendingGiphyBot.Jobs
{
    public abstract class Job : IDisposable
    {
        readonly Timer _Timer;
        readonly string _Name;
        protected ILogger Logger { get; }
        protected IGlobalConfig GlobalConfig { get; }
        protected DiscordSocketClient DiscordClient { get; }
        protected int Interval { get; }
        protected Time Time { get; }
        readonly ushort _IntervalOffsetSeconds;
        protected Job(IGlobalConfig globalConfig, ILogger logger, SubJobConfig subJobConfig)
        {
            GlobalConfig = globalConfig;
            DiscordClient = GlobalConfig.DiscordClient;
            Interval = subJobConfig.Interval;
            Time = subJobConfig.Time;
            _IntervalOffsetSeconds = subJobConfig.IntervalOffsetSeconds;
            Logger = logger;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
            _Name = $"{GetType().Name} {Interval} {Time}";
        }
        async void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            Logger.Info($"{_Name} fired.");
            await Logger.SwallowAsync(Run);
            Logger.Info($"{_Name} success.");
            StartTimerWithCloseInterval();
        }
        internal void StartTimerWithCloseInterval()
        {
            var now = DateTime.Now;
            var nextElapse = DetermineNextElapse(now);
            var offset = TimeSpan.FromSeconds(GlobalConfig.Config.IntervalOffsetSeconds + _IntervalOffsetSeconds);
            _Timer.Interval = (nextElapse - now + offset).TotalMilliseconds;
            _Timer.Start();
            Logger.Trace($"{_Name} next elapse: {nextElapse}.");
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
                    throw new UnexpectedTimeException(Time);
            }
        }
        int DetermineDifference(int component) => Interval - component % Interval;
        protected abstract Task Run();
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Timer")]
        public void Dispose()
        {
            _Timer?.Stop();
            _Timer?.Dispose();
        }
    }
}
