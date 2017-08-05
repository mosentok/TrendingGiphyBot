using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class ReportStatsJob : Job
    {
        public ReportStatsJob(IServiceProvider services, ILogger logger, int interval, Time time) : base(services, logger, interval, time) { }
        protected override async Task Run()
        {
        }
    }
}
