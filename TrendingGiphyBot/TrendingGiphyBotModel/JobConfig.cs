﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrendingGiphyBotModel
{
    public class JobConfig
    {
        [Key]
        public decimal ChannelId { get; set; }
        public int? Interval { get; set; }
        public string Time { get; set; }
        public string RandomSearchString { get; set; }
        public short? MinQuietHour { get; set; }
        public short? MaxQuietHour { get; set; }
        public int? IntervalMinutes { get; set; }
        public string Prefix { get; set; }
        public virtual ICollection<UrlHistory> UrlHistories { get; set; }
    }
}
