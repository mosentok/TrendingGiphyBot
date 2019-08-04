using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class PostGifsHelper
    {
        readonly IGifPostingHelper _GifPostingHelper;
        public PostGifsHelper(IGifPostingHelper gifPostingHelper)
        {
            _GifPostingHelper = gifPostingHelper;
        }
        public async Task RunAsync(DateTime now, List<int> allValidMinutes, string giphyRandomEndpoint, string botToken)
        {
            await _GifPostingHelper.LogInAsync(botToken);
            var totalMinutes = _GifPostingHelper.DetermineTotalMinutes(now);
            var currentValidMinutes = _GifPostingHelper.DetermineCurrentValidMinutes(totalMinutes, allValidMinutes);
            var pendingContainers = await _GifPostingHelper.GetContainers(now.Hour, currentValidMinutes);
            var historyContainers = await _GifPostingHelper.BuildHistoryContainers(pendingContainers, giphyRandomEndpoint);
            var insertedContainers = await _GifPostingHelper.InsertHistories(historyContainers);
            var channelResult = await _GifPostingHelper.BuildChannelContainers(insertedContainers);
            var gifPostingResult = await _GifPostingHelper.PostGifs(channelResult.ChannelContainers);
            if (channelResult.Errors.Any())
                await _GifPostingHelper.DeleteErrorHistories(channelResult.Errors);
            if (channelResult.ChannelsToDelete.Any())
                await _GifPostingHelper.DeleteJobConfigs(channelResult.ChannelsToDelete);
            if (gifPostingResult.Errors.Any())
                await _GifPostingHelper.DeleteErrorHistories(gifPostingResult.Errors);
            if (gifPostingResult.ChannelsToDelete.Any())
                await _GifPostingHelper.DeleteJobConfigs(gifPostingResult.ChannelsToDelete);
            await _GifPostingHelper.LogOutAsync();
        }
    }
}
