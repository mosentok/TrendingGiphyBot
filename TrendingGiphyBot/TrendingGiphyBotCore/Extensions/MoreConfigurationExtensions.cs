﻿using Microsoft.Extensions.Configuration;

namespace TrendingGiphyBotCore.Extensions
{
    public static class MoreConfigurationExtensions
    {
        public static T Get<T>(this IConfiguration config, string sectionName) => config.GetSection(sectionName).Get<T>();
    }
}
