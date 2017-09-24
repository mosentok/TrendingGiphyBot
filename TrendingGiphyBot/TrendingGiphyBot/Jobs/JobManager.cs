using System;
using System.Collections.Generic;
using System.Linq;
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
            _Jobs.Add(new RefreshImagesJob(_GlobalConfig, Config.RefreshImageJobConfig));
            _Jobs.Add(new DeleteOldUrlCachesJob(_GlobalConfig, Config.DeleteOldUrlCachesJobConfig));
            _Jobs.Add(new DeleteOldUrlHistoriesJob(_GlobalConfig, Config.DeleteOldUrlHistoriesJobConfig));
            _Jobs.ForEach(s => s.StartTimerWithCloseInterval());
        }
        List<PostImageJob> BuildPostImageJobs() => BuildPostImageJobs(Config.ValidSeconds, Time.Second, Time.Seconds)
            .Concat(BuildPostImageJobs(Config.ValidMinutes, Time.Minute, Time.Minutes))
            .Concat(BuildPostImageJobs(Config.ValidHours, Time.Hour, Time.Hours)).ToList();
        IEnumerable<PostImageJob> BuildPostImageJobs(List<int> validIntervals, params Time[] times) => times.SelectMany(t => validIntervals.Select(s => new PostImageJob(_GlobalConfig, s, t)));
        public void Dispose()
        {
            _Jobs.ForEach(s => s?.Dispose());
        }
    }
}
