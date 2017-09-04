using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class DeleteOldUrlHistoriesJob : Job
    {
        internal DeleteOldUrlHistoriesJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            var oldestDate = DateTime.Now.AddDays(-GlobalConfig.Config.UrlHistoriesMaxDaysOld);
            await GlobalConfig.UrlHistoryDal.DeleteOlderThan(oldestDate);
        }
    }
}
