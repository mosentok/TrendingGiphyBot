using System;
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
            using (var context = new TrendingGiphyBotContext(connectionString))
            using (var giphyHelper = new GiphyHelper())
            using (var discordHelper = new DiscordHelper(botToken))
            {
                var gifPostingHelper = new GifPostingHelper(log, context, giphyHelper, discordHelper);
                var postGifsFunction = new PostGifsFunction(gifPostingHelper);
                await postGifsFunction.RunAsync();
            }
        }
        readonly IGifPostingHelper _GifPostingHelper;
        public PostGifsFunction(IGifPostingHelper gifPostingHelper)
        {
            _GifPostingHelper = gifPostingHelper;
        }
        public async Task RunAsync()
        {
            await _GifPostingHelper.LogInAsync();
            var pendingContainers = await _GifPostingHelper.GetContainers();
            var historyContainers = await _GifPostingHelper.BuildHistoryContainers(pendingContainers);
            var insertedContainers = await _GifPostingHelper.InsertHistories(historyContainers);
            var channelResult = await _GifPostingHelper.BuildChannelContainers(insertedContainers);
            var gifPostingResult = await _GifPostingHelper.PostGifs(channelResult.ChannelContainers);
            var allHistoryErrors = channelResult.Errors.Concat(gifPostingResult.Errors).ToList();
            if (allHistoryErrors.Any())
                await _GifPostingHelper.DeleteErrorHistories(allHistoryErrors);
            if (gifPostingResult.ChannelsToDelete.Any())
                await _GifPostingHelper.DeleteJobConfigs(gifPostingResult.ChannelsToDelete);
            await _GifPostingHelper.LogOutAsync();
        }
    }
}
