using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Modules
{
    [Group("Owner")]
    public class OwnerModule : ModuleBase
    {
        readonly ILogger _Logger;
        readonly IGlobalConfig _GlobalConfig;
        public OwnerModule(IServiceProvider services)
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(RefreshConfig))]
        public async Task RefreshConfig()
        {
            if (Context.User.Id == _GlobalConfig.Config.OwnerId)
            {
                await _GlobalConfig.RefreshConfig();
                await TryReplyAsync("Done.");
            }
        }
        [Command(nameof(SetGame))]
        public async Task SetGame(params string[] literalSentenceToSetAsGame)
        {
            if (Context.User.Id == _GlobalConfig.Config.OwnerId)
            {
                var game = DetermineGame(literalSentenceToSetAsGame);
                if (!string.IsNullOrEmpty(game))
                {
                    await _GlobalConfig.DiscordClient.SetGameAsync(game);
                    await TryReplyAsync("Done.");
                }
            }
        }
        async Task TryReplyAsync(string message)
        {
            try
            {
                await ReplyAsync(message);
            }
            catch (HttpException httpException) when (_GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                await _GlobalConfig.MessageHelper.SendMessageToUser(Context, message, null);
            }
        }
        static string DetermineGame(string[] words)
        {
            if (words.Any())
                return string.Join(" ", words);
            return null;
        }
    }
}
