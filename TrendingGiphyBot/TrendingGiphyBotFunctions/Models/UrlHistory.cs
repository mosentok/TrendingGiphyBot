﻿using System;
using System.Collections.Generic;

namespace TrendingGiphyBotFunctions.Models
{
    public partial class UrlHistory
    {
        public decimal ChannelId { get; set; }
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
    }
}
