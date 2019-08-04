using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TrendingGiphyBotFunctions.Wrappers;

[assembly: FunctionsStartup(typeof(TrendingGiphyBotFunctions.Startup))]

namespace TrendingGiphyBotFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IDiscordWrapper, DiscordWrapper>();
            builder.Services.AddSingleton<IGiphyWrapper, GiphyWrapper>();
            builder.Services.AddSingleton<IStatWrapper, StatWrapper>();
        }
    }
}
