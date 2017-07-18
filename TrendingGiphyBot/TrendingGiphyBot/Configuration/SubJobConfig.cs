﻿using Newtonsoft.Json;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Configuration
{
    class SubJobConfig
    {
        [JsonRequired]
        public int Interval { get; set; }
        [JsonRequired]
        public Time Time { get; set; }
    }
}