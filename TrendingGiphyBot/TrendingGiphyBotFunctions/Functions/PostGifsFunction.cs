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
            await _PostGifsHelper.LogInAsync();
            var pendingContainers = await _PostGifsHelper.GetContainers(log);
            var historyContainers = await _PostGifsHelper.BuildHistoryContainers(pendingContainers, log);
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
