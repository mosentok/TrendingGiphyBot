using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotCore.Extensions;

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
            var warningResponses = _Config.Get<List<string>>("WarningResponses");
            try
            {
                await ReplyAsync(message);
            }
            catch (HttpException httpException) when (warningResponses.Any(httpException.Message.EndsWith))
            {
                _Logger.LogWarning(httpException.Message);
            }
        }
    }
}
