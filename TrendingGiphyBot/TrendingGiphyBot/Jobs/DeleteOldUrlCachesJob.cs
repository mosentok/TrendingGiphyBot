using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Jobs
{
    class DeleteOldUrlCachesJob : Job
    {
        internal DeleteOldUrlCachesJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            var oldestDate = DateTime.Now.AddDays(-GlobalConfig.Config.UrlCachesMaxDaysOld);
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                await entities.DeleteUrlCachesOlderThan(oldestDate);
        }
    }
}
