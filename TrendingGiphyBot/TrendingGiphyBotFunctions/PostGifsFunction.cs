using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class PostGifsFunction
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
            using (var context = new TrendingGiphyBotContext(connectionString))
            using (var giphyHelper = new GiphyHelper())
            using (var discordHelper = new DiscordHelper(botToken))
            {
                var gifPostingHelper = new GifPostingHelper(log, context, giphyHelper, discordHelper);
                var postGifsFunction = new PostGifsFunction(gifPostingHelper);
                await postGifsFunction.RunAsync(now, allValidMinutes, giphyRandomEndpoint, warningResponses);
            }
        }
        readonly IGifPostingHelper _GifPostingHelper;
        public PostGifsFunction(IGifPostingHelper gifPostingHelper)
        {
            _GifPostingHelper = gifPostingHelper;
        }
        public async Task RunAsync(DateTime now, List<int> allValidMinutes, string giphyRandomEndpoint, List<string> warningResponses)
        {
            await _GifPostingHelper.LogInAsync();
            var pendingContainers = await _GifPostingHelper.GetContainers(now, allValidMinutes);
            var historyContainers = await _GifPostingHelper.BuildHistoryContainers(pendingContainers, giphyRandomEndpoint);
            var insertedContainers = await _GifPostingHelper.InsertHistories(historyContainers);
            var channelResult = await _GifPostingHelper.BuildChannelContainers(insertedContainers);
            var gifPostingResult = await _GifPostingHelper.PostGifs(channelResult.ChannelContainers, warningResponses);
            if (channelResult.Errors.Any())
                await _GifPostingHelper.DeleteErrorHistories(channelResult.Errors);
            if (gifPostingResult.Errors.Any())
                await _GifPostingHelper.DeleteErrorHistories(gifPostingResult.Errors);
            if (gifPostingResult.ChannelsToDelete.Any())
                await _GifPostingHelper.DeleteJobConfigs(gifPostingResult.ChannelsToDelete);
            await _GifPostingHelper.LogOutAsync();
        }
    }
}
