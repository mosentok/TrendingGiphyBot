using Discord.Commands;
using System;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(TrendingGiphyBot))]
    [Alias("TGB")]
    public class MainModule : ModuleBase
    {
        readonly IGlobalConfig _GlobalConfig;
        public MainModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
        }
    }
}
