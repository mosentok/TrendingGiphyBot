﻿using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Timers;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Exceptions;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    abstract class Job : IDisposable
    {
        readonly Timer _Timer;
        protected ILogger Logger { get; }
        protected IGlobalConfig GlobalConfig { get; }
        protected DiscordSocketClient DiscordClient { get; }
        internal int Interval { get; }
        internal Time Time { get; }
        protected DateTime NextElapse { get; private set; }
        protected string Name { get; }
        protected Job(IServiceProvider services, ILogger logger, int interval, string time) : this(services, logger, interval, time.ToTime()) { }
        protected Job(IServiceProvider services, ILogger logger, int interval, Time time)
        {
            GlobalConfig = services.GetRequiredService<IGlobalConfig>();
            DiscordClient = GlobalConfig.DiscordClient;
            Interval = interval;
            Time = time;
            Logger = logger;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
            Name = GetType().Name;
        }
        async void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            Logger.Info($"{Name} fired.");
            await Run();
            Logger.Info($"{Name} success.");
            StartTimerWithCloseInterval();
        }
        internal void StartTimerWithCloseInterval()
        {
            var now = DateTime.Now;
            NextElapse = DetermineNextElapse(now);
            var interval = (NextElapse - now).TotalMilliseconds;
            _Timer.Interval = interval;
            _Timer.Start();
            Logger.Debug(TimerStartedLog);
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
        protected abstract string TimerStartedLog { get; }
        protected internal abstract Task Run();
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_Timer")]
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
