using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Modules
{
    [Group("Owner")]
    public class OwnerModule : ModuleBase
    {
        readonly IGlobalConfig _GlobalConfig;
        public OwnerModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(RefreshConfig))]
        public async Task RefreshConfig()
        {
            if (Context.User.Id == _GlobalConfig.Config.OwnerId)
            {
                _GlobalConfig.RefreshConfig();
                await ReplyAsync("Done.");
            }
        }
        [Command(nameof(SetGame))]
        public async Task SetGame(params string[] searchValues)
        {
            if (Context.User.Id == _GlobalConfig.Config.OwnerId)
            {
                var game = DetermineGame(searchValues);
                if (!string.IsNullOrEmpty(game))
                {
                    await _GlobalConfig.DiscordClient.SetGameAsync(game);
                    await ReplyAsync("Done.");
                }
            }
        }
        static string DetermineGame(string[] searchValues)
        {
            if (searchValues.Any())
                return searchValues.Join(" ");
            return null;
        }
    }
}
