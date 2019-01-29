using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TrendingGiphyBotCore.Modules
{
    [Group("Owner")]
    public class OwnerModule : ModuleBase
    {
        readonly ILogger _Logger;
        readonly IConfiguration _Config;
        readonly DiscordSocketClient _DiscordClient;
        public OwnerModule(IServiceProvider services)
        {
            _Logger = services.GetService<ILogger<OwnerModule>>();
            _Config = services.GetService<IConfiguration>();
            _DiscordClient = services.GetService<DiscordSocketClient>();
        }
        [Command(nameof(SetGame))]
        public async Task SetGame([Remainder] string literalSentenceToSetAsGame)
        {
            var ownerId = _Config.GetValue<ulong>("OwnerId");
            if (Context.User.Id == ownerId && !string.IsNullOrEmpty(literalSentenceToSetAsGame))
            {
                await _DiscordClient.SetGameAsync(literalSentenceToSetAsGame);
                await TryReplyAsync("Done.");
            }
        }
        async Task TryReplyAsync(string message)
        {
            try
            {
                await ReplyAsync(message);
            }
            //TODO move these to config
            catch (HttpException httpException) when (httpException.Message.EndsWith("Missing Access") ||
                                                      httpException.Message.EndsWith("Missing Permissions"))
            {
                _Logger.LogWarning(httpException.Message);
            }
        }
    }
}
