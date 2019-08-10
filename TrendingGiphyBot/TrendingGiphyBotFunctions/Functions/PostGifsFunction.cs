using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;

namespace TrendingGiphyBotFunctions.Functions
{
    public class PostGifsFunction
    {
        readonly IPostGifsHelper _PostGifsHelper;
        public PostGifsFunction(IPostGifsHelper postGifsHelper)
        {
            _PostGifsHelper = postGifsHelper;
        }
        [FunctionName(nameof(PostGifsFunction))]
        public async Task Run([TimerTrigger("%PostGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
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
            await _PostGifsHelper.LogInAsync(botToken);
            var totalMinutes = _PostGifsHelper.DetermineTotalMinutes(now);
            var currentValidMinutes = _PostGifsHelper.DetermineCurrentValidMinutes(totalMinutes, allValidMinutes);
            var pendingContainers = await _PostGifsHelper.GetContainers(now.Hour, currentValidMinutes, log);
            var historyContainers = await _PostGifsHelper.BuildHistoryContainers(pendingContainers, giphyRandomEndpoint, log);
            var insertedContainers = await _PostGifsHelper.InsertHistories(historyContainers, log);
            var channelResult = await _PostGifsHelper.BuildChannelContainers(insertedContainers, log);
            var gifPostingResult = await _PostGifsHelper.PostGifs(channelResult.ChannelContainers, log);
            if (channelResult.Errors.Any())
                await _PostGifsHelper.DeleteErrorHistories(channelResult.Errors, log);
            if (channelResult.ChannelsToDelete.Any())
                await _PostGifsHelper.DeleteJobConfigs(channelResult.ChannelsToDelete, log);
            if (gifPostingResult.Errors.Any())
                await _PostGifsHelper.DeleteErrorHistories(gifPostingResult.Errors, log);
            if (gifPostingResult.ChannelsToDelete.Any())
                await _PostGifsHelper.DeleteJobConfigs(gifPostingResult.ChannelsToDelete, log);
            await _PostGifsHelper.LogOutAsync();
        }
    }
}
