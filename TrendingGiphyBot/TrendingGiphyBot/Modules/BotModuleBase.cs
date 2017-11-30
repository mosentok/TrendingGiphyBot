using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Modules
{
    public abstract class BotModuleBase : ModuleBase
    {
        readonly ILogger _Logger;
        protected IGlobalConfig GlobalConfig { get; }
        protected BotModuleBase(IServiceProvider services, ILogger logger)
        {
            _Logger = logger;
            GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        protected async Task HelpMessageReplyAsync()
        {
            var helpMessageEmbed = GlobalConfig.BuildEmbedFromConfig(GlobalConfig.Config.HelpMessage);
            await TryReplyAsync(helpMessageEmbed);
        }
        protected async Task TryReplyAsync(string message) => await TryReplyAsync(message, null);
        protected async Task TryReplyAsync(EmbedBuilder embedBuilder) => await TryReplyAsync(string.Empty, embedBuilder);
        async Task TryReplyAsync(string message, EmbedBuilder embedBuilder)
        {
            try
            {
                await ReplyAsync(message, embed: embedBuilder);
            }
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                await GlobalConfig.MessageHelper.SendMessageToUser(Context, message, embedBuilder);
            }
        }
        protected override void BeforeExecute(CommandInfo command) => _Logger.Trace($"Calling {command.Name}.");
        protected override void AfterExecute(CommandInfo command) => _Logger.Trace($"{command.Name} done.");
    }
}
