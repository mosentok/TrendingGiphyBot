using System;
using System.Collections.Generic;
using System.Linq;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Extensions
{
    static class JobConfigExtensions
    {
        internal static bool IsInQuietHours(this JobConfig jobConfig)
        {
            var nowHour = DateTime.Now.Hour;
            if (jobConfig.MinQuietHour.HasValue && jobConfig.MaxQuietHour.HasValue)
            {
                IEnumerable<int> quietHours;
                if (jobConfig.MinQuietHour.Value == jobConfig.MaxQuietHour.Value)
                    quietHours = new List<int> { jobConfig.MinQuietHour.Value };
                else if (jobConfig.MinQuietHour.Value < jobConfig.MaxQuietHour.Value)
                    quietHours = Enumerable.Range(jobConfig.MinQuietHour.Value, jobConfig.MaxQuietHour.Value - jobConfig.MinQuietHour.Value + 1);
                else
                {
                    var minHours = Enumerable.Range(jobConfig.MinQuietHour.Value, 24 - jobConfig.MinQuietHour.Value);
                    var maxHours = Enumerable.Range(0, jobConfig.MaxQuietHour.Value + 1);
                    quietHours = minHours.Concat(maxHours);
                }
                return quietHours.Contains(nowHour);
            }
            return false;
        }
    }
}
