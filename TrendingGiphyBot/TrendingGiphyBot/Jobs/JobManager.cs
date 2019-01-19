using System;
using System.Collections.Generic;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    public class JobManager : IDisposable
    {
        readonly IGlobalConfig _GlobalConfig;
        readonly IFunctionHelper _FunctionHelper;
        Config Config => _GlobalConfig.Config;
        readonly List<Job> _Jobs = new List<Job>();
        internal JobManager(IGlobalConfig globalConfig, IFunctionHelper functionHelper)
        {
            _GlobalConfig = globalConfig;
            _FunctionHelper = functionHelper;
        }
        public void Ready()
        {
            _Jobs.ForEach(s => s?.Dispose());
            _Jobs.Clear();
            if (Config.PostImageJobConfig.IsEnabled)
                _Jobs.Add(new PostImageJob(_GlobalConfig, _FunctionHelper, Config.PostImageJobConfig));
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
