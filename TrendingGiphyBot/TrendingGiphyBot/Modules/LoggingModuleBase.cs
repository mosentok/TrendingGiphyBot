using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Modules
{
    public abstract class LoggingModuleBase : ModuleBase
    {
        readonly ILogger _Logger;
        protected IGlobalConfig GlobalConfig { get; }
        protected LoggingModuleBase(IServiceProvider services, ILogger logger)
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
        protected async Task TryReplyAsync(string message, EmbedBuilder embedBuilder)
        {
            try
            {
                await ReplyAsync(message, embed: embedBuilder);
            }
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                await SendMessageToUser(message, embedBuilder);
            }
        }
        async Task SendMessageToUser(string message, EmbedBuilder embedBuilder)
        {
            if (!string.IsNullOrEmpty(GlobalConfig.Config.FailedReplyDisclaimer))
            {
                var failedReplyDisclaimer = string.Format(GlobalConfig.Config.FailedReplyDisclaimer, Context.Channel.Name);
                if (embedBuilder != null)
                    await SendMessageToUserWithDisclaimerFooter(message, failedReplyDisclaimer, embedBuilder);
                else
                    await SendMessageToUserWithDisclaimerText(message, failedReplyDisclaimer);
            }
            else
                await Context.User.SendMessageAsync(message, embed: embedBuilder);
        }
        async Task SendMessageToUserWithDisclaimerFooter(string message, string failedReplyDisclaimer, EmbedBuilder embedBuilder)
        {
            embedBuilder.Footer = new EmbedFooterBuilder()
                .WithText(failedReplyDisclaimer);
            await Context.User.SendMessageAsync(message, embed: embedBuilder);
        }
        async Task SendMessageToUserWithDisclaimerText(string message, string failedReplyDisclaimer)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(message);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"*{failedReplyDisclaimer}*");
            await Context.User.SendMessageAsync(stringBuilder.ToString().TrimEnd());
        }
        protected override void BeforeExecute(CommandInfo command) => _Logger.Trace($"Calling {command.Name}.");
        protected override void AfterExecute(CommandInfo command) => _Logger.Trace($"{command.Name} done.");
    }
}
