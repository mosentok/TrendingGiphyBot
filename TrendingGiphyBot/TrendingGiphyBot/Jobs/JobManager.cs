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
            _Jobs.Add(new PostImageJob(_GlobalConfig, new SubJobConfig(10, Time.Minutes)));
            _Jobs.Add(new RefreshImagesJob(_GlobalConfig, Config.RefreshImageJobConfig));
            _Jobs.Add(new DeleteOldUrlCachesJob(_GlobalConfig, Config.DeleteOldUrlCachesJobConfig));
            _Jobs.Add(new DeleteOldUrlHistoriesJob(_GlobalConfig, Config.DeleteOldUrlHistoriesJobConfig));
            _Jobs.ForEach(s => s.StartTimerWithCloseInterval());
        }
        public void Dispose()
        {
            _Jobs.ForEach(s => s?.Dispose());
        }
    }
}
