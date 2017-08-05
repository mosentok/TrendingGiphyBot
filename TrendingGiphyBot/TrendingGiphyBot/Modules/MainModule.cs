﻿using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(TrendingGiphyBot))]
    [Alias("TGB")]
    public class MainModule : LoggingModuleBase
    {
        public MainModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()){}
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {GlobalConfig.Config.GitHubUrl} for help!");
        }
    }
}
