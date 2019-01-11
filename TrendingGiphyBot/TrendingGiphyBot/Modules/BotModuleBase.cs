using System;
using System.Threading.Tasks;
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
        protected async Task SendDeprecatedCommandMessage()
        {
            try
            {
                await ReplyAsync(GlobalConfig.Config.DeprecatedCommandMessage);
            }
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                await GlobalConfig.MessageHelper.SendMessageToUser(Context, GlobalConfig.Config.DeprecatedCommandMessage, null);
            }
        }
    }
}
