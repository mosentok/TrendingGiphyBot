using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class DeleteOldUrlHistoriesJob : Job
    {
        internal DeleteOldUrlHistoriesJob(IGlobalConfig globalConfig, int interval, Time time) : base(globalConfig, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            var oldestDate = DateTime.Now.AddDays(-GlobalConfig.Config.UrlHistoriesMaxDaysOld);
            await GlobalConfig.UrlHistoryDal.DeleteOlderThan(oldestDate);
        }
    }
}
