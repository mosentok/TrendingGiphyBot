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
            try
            {
                await ReplyAsync(string.Empty, embed: helpMessageEmbed);
            }
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                if (!string.IsNullOrEmpty(GlobalConfig.Config.HelpMessage.FooterText))
                {
                    var footerWithChannelName = string.Format(GlobalConfig.Config.HelpMessage.FooterText, Context.Channel.Name);
                    helpMessageEmbed.Footer = new EmbedFooterBuilder()
                        .WithText(footerWithChannelName);
                }
                await Context.User.SendMessageAsync(string.Empty, embed: helpMessageEmbed);
            }
        }
        protected override void BeforeExecute(CommandInfo command)
        {
            _Logger.Trace($"Calling {command.Name}.");
        }
        protected override void AfterExecute(CommandInfo command)
        {
            _Logger.Trace($"{command.Name} done.");
        }
    }
}
