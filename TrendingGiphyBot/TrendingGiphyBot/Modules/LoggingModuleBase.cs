using System;
using Discord.Commands;
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
