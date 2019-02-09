using System;
using System.Collections.Generic;

namespace TrendingGiphyBotMigrator.OriginalModel
{
    public partial class Time
    {
        public Time()
        {
            JobConfig = new HashSet<JobConfig>();
        }

        public string Value { get; set; }

        public virtual ICollection<JobConfig> JobConfig { get; set; }
    }
}
