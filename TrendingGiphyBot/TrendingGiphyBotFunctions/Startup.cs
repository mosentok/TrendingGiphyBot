using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

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
            builder.Services.AddSingleton<ITrendingGiphyBotContext>(_ =>
            {
                var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
                return new TrendingGiphyBotContext(connectionString);
            });
            builder.Services.AddSingleton<IPostGifsHelper>(s =>
            {
                var warningResponses = Environment.GetEnvironmentVariable("WarningResponses").Split(',', options: StringSplitOptions.RemoveEmptyEntries).ToList();
                return new PostGifsHelper(s, warningResponses);
            });
        }
    }
}
