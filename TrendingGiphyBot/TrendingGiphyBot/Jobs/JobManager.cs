using System;
using System.Collections.Generic;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    public class JobManager : IDisposable
    {
        readonly IGlobalConfig _GlobalConfig;
        Config Config => _GlobalConfig.Config;
        readonly List<Job> _Jobs = new List<Job>();
        internal JobManager(IGlobalConfig globalConfig)
        {
            _GlobalConfig = globalConfig;
        }
        public void Ready()
        {
            _Jobs.ForEach(s => s?.Dispose());
            _Jobs.Clear();
            var postImageJobs = BuildPostImageJobs();
            _Jobs.AddRange(postImageJobs);
            _Jobs.Add(new RefreshImagesJob(_GlobalConfig, Config.RefreshImageJobConfig.Interval, Config.RefreshImageJobConfig.Time));
            _Jobs.Add(new DeleteOldUrlCachesJob(_GlobalConfig, Config.DeleteOldUrlCachesJobConfig.Interval, Config.DeleteOldUrlCachesJobConfig.Time));
            _Jobs.Add(new DeleteOldUrlHistoriesJob(_GlobalConfig, Config.DeleteOldUrlHistoriesJobConfig.Interval, Config.DeleteOldUrlHistoriesJobConfig.Time));
            _Jobs.ForEach(s => s.StartTimerWithCloseInterval());
        }
        List<PostImageJob> BuildPostImageJobs()
        {
            var postImageJobs = new List<PostImageJob>();
            //TODO all these foreachs bug me
            foreach (var second in Config.ValidSeconds)
            {
                AddJob(postImageJobs, second, Time.Second);
                AddJob(postImageJobs, second, Time.Seconds);
            }
            foreach (var minute in Config.ValidMinutes)
            {
                AddJob(postImageJobs, minute, Time.Minute);
                AddJob(postImageJobs, minute, Time.Minutes);
            }
            foreach (var hour in Config.ValidHours)
            {
                AddJob(postImageJobs, hour, Time.Hour);
                AddJob(postImageJobs, hour, Time.Hours);
            }
            return postImageJobs;
        }
        void AddJob(List<PostImageJob> postImageJobs, int interval, Time time)
        {
            postImageJobs.Add(new PostImageJob(_GlobalConfig, interval, time));
        }
        public void Dispose()
        {
            _Jobs.ForEach(s => s?.Dispose());
        }
    }
}
