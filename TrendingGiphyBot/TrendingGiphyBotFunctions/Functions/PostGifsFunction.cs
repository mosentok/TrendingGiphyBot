using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Functions
{
    public static class PostGifsFunction
    {
        [FunctionName(nameof(PostGifsFunction))]
        public static async Task Run([TimerTrigger("%PostGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var botToken = Environment.GetEnvironmentVariable("BotToken");
            var hourOffsetString = Environment.GetEnvironmentVariable("HourOffset");
            var hourOffset = int.Parse(hourOffsetString);
            var now = DateTime.Now.AddHours(-hourOffset);
            var validMinutesString = Environment.GetEnvironmentVariable("ValidMinutes");
            var validMinutes = validMinutesString.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var validHoursString = Environment.GetEnvironmentVariable("ValidHours");
            var validHours = validHoursString.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var validHoursAsMinutes = validHours.Select(s => s * 60);
            var allValidMinutes = validMinutes.Concat(validHoursAsMinutes).ToList();
            var giphyRandomEndpoint = Environment.GetEnvironmentVariable("GiphyRandomEndpoint");
            var warningResponses = Environment.GetEnvironmentVariable("WarningResponses").Split(',', options: StringSplitOptions.RemoveEmptyEntries).ToList();
            var logWrapper = new LoggerWrapper(log);
            using (var context = new TrendingGiphyBotContext(connectionString))
            using (var giphyWrapper = new GiphyWrapper())
            using (var discordWrapper = new DiscordWrapper())
            {
                var gifPostingHelper = new GifPostingHelper(logWrapper, context, giphyWrapper, discordWrapper, warningResponses);
                var postGifsHelper = new PostGifsHelper(gifPostingHelper);
                await postGifsHelper.RunAsync(now, allValidMinutes, giphyRandomEndpoint, botToken);
            }
        }
    }
}
