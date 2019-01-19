using System;
using System.Collections.Generic;

namespace TrendingGiphyBotFunctions.Models
{
    public partial class Time
    {
        public Time()
        {
            JobConfig = new HashSet<JobConfig>();
        }

        public string Value { get; set; }

        public ICollection<JobConfig> JobConfig { get; set; }
    }
}
