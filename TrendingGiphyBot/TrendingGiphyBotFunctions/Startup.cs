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
            builder.Services.AddSingleton<IDiscordWrapper>(_ =>
            {
                var botToken = Environment.GetEnvironmentVariable("BotToken");
                return new DiscordWrapper(botToken);
            });
            builder.Services.AddSingleton<IGiphyWrapper>(_ =>
            {
                var trendingEndpoint = Environment.GetEnvironmentVariable("GiphyTrendingEndpoint");
                var giphyRandomEndpoint = Environment.GetEnvironmentVariable("GiphyRandomEndpoint");
                return new GiphyWrapper(trendingEndpoint, giphyRandomEndpoint);
            });
            builder.Services.AddSingleton<IStatWrapper, StatWrapper>();
            builder.Services.AddSingleton<ITrendingGiphyBotContext>(_ =>
            {
                var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
                var urlCachesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlCachesMaxDaysOld");
                var urlCachesMaxDaysOld = int.Parse(urlCachesMaxDaysOldString);
                var urlHistoriesMaxDaysOldString = Environment.GetEnvironmentVariable("UrlHistoriesMaxDaysOld");
                var urlHistoriesMaxDaysOld = int.Parse(urlHistoriesMaxDaysOldString);
                return new TrendingGiphyBotContext(connectionString, urlCachesMaxDaysOld, urlHistoriesMaxDaysOld);
            });
            builder.Services.AddSingleton<IPostGifsHelper>(s =>
            {
                var hourOffsetString = Environment.GetEnvironmentVariable("HourOffset");
                var hourOffset = int.Parse(hourOffsetString);
                var now = DateTime.Now.AddHours(-hourOffset);
                var totalMinutes = DetermineTotalMinutes();
                var validMinutesString = Environment.GetEnvironmentVariable("ValidMinutes");
                var validMinutes = validMinutesString.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var validHoursString = Environment.GetEnvironmentVariable("ValidHours");
                var validHours = validHoursString.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var validHoursAsMinutes = validHours.Select(t => t * 60);
                var allValidMinutes = validMinutes.Concat(validHoursAsMinutes).ToList();
                var currentValidMinutes = allValidMinutes.Where(t => totalMinutes % t == 0).ToList();
                var warningResponses = Environment.GetEnvironmentVariable("WarningResponses").Split(',', options: StringSplitOptions.RemoveEmptyEntries).ToList();
                return new PostGifsHelper(s, warningResponses, now, currentValidMinutes);
                int DetermineTotalMinutes()
                {
                    if (now.Hour == 0 && now.Minute == 0)
                        return 24 * 60;
                    return now.Hour * 60 + now.Minute;
                }
            });
        }
    }
}
