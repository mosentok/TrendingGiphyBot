using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class DeleteOldUrlCachesJob : Job
    {
        internal DeleteOldUrlCachesJob(IGlobalConfig globalConfig, int interval, Time time) : base(globalConfig, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            var oldestDate = DateTime.Now.AddDays(-GlobalConfig.Config.UrlCachesMaxDaysOld);
            await GlobalConfig.UrlCacheDal.DeleteOlderThan(oldestDate);
        }
    }
}
