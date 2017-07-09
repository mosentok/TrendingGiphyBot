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
            var now = DateTime.Now;
            var nextElapse = DetermineNextElapse(now);
            var interval = (nextElapse - now).TotalMilliseconds;
            _Timer.Interval = interval;
            _Timer.Start();
        }
        DateTime DetermineNextElapse(DateTime now)
        {
            int difference;
            switch (JobConfig.Time)
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
                    throw new InvalidOperationException();
            }
        }
        int DetermineDifference(int component)
        {
            return JobConfig.Interval - component % JobConfig.Interval;
        }
        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
