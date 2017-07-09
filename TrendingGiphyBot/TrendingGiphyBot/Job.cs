using System;
using System.Threading.Tasks;
using System.Timers;

namespace TrendingGiphyBot
{
    class Job : IDisposable
    {
        internal JobConfig JobConfig { get; private set; }
        public event Func<Task> WorkToDo;
        Timer _Timer;
        public Job(JobConfig jobConfig)
        {
            JobConfig = jobConfig;
            _Timer = new Timer();
            _Timer.Elapsed += Elapsed;
            StartTimerWithCloseInterval();
        }
        void Elapsed(object sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            WorkToDo();
            StartTimerWithCloseInterval();
        }
        internal void Restart(JobConfig jobConfig)
        {
            _Timer.Stop();
            JobConfig = jobConfig;
            StartTimerWithCloseInterval();
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
            switch (JobConfig.Time)
            {
                case Time.Hours:
                    return (int)TimeSpan.FromHours(JobConfig.Interval).TotalSeconds;
                case Time.Minutes:
                    return (int)TimeSpan.FromMinutes(JobConfig.Interval).TotalSeconds;
                case Time.Seconds:
                    return (int)TimeSpan.FromSeconds(JobConfig.Interval).TotalSeconds;
                default:
                    throw new InvalidOperationException($"{JobConfig.Time} is an invalid {nameof(Time)}.");
            }
        }
        static int DetermineDifferenceSeconds(int runEveryXSeconds)
        {
            return runEveryXSeconds - DateTime.Now.Second % runEveryXSeconds;
        }
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
