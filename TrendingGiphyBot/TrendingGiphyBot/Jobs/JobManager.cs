using System;
using System.Collections.Generic;
using TrendingGiphyBot.Configuration;

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
            if (Config.PostImageJobConfig.IsEnabled)
                _Jobs.Add(new PostImageJob(_GlobalConfig, Config.PostImageJobConfig));
            if (Config.RefreshImageJobConfig.IsEnabled)
                _Jobs.Add(new RefreshImagesJob(_GlobalConfig, Config.RefreshImageJobConfig));
            if (Config.DeleteOldUrlCachesJobConfig.IsEnabled)
                _Jobs.Add(new DeleteOldUrlCachesJob(_GlobalConfig, Config.DeleteOldUrlCachesJobConfig));
            if (Config.DeleteOldUrlHistoriesJobConfig.IsEnabled)
                _Jobs.Add(new DeleteOldUrlHistoriesJob(_GlobalConfig, Config.DeleteOldUrlHistoriesJobConfig));
            _Jobs.ForEach(s => s.StartTimerWithCloseInterval());
        }
        public void Dispose()
        {
            _Jobs.ForEach(s => s?.Dispose());
        }
    }
}
