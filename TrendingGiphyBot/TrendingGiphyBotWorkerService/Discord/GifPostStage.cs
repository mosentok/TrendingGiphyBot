using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy;

namespace TrendingGiphyBotWorkerService.Discord;

public class GifPostStage(IServiceScopeFactory _serviceScopeFactory, IGifCache _gifCache) : IGifPostStage
{
	readonly Dictionary<ulong, GiphyData> _channelGifPostStage = [];

	public void Evict(ulong channelId) => _channelGifPostStage.Remove(channelId);

	public void Refresh()
	{
		using var scope = _serviceScopeFactory.CreateScope();

		var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();
		var activeChannels = trendingGiphyBotDbContext.ChannelSettings.Where(s => s.HowOften != null);

		foreach (var channel in activeChannels)
		{
			var channelIsAlreadyStaged = _channelGifPostStage.ContainsKey(channel.ChannelId);

			if (channelIsAlreadyStaged)
				continue;

			var firstUnseenGif = DetermineFirstUnseenGif(channel);

			if (firstUnseenGif is null)
			{
				// TODO this should be an enum or something
				if (channel.GifPostingBehavior == "POST WITH RANDOM GIF TODO")
				{
					// TODO get and stage random gif based on GifKeyword
				}

				continue;
			}

			_channelGifPostStage[channel.ChannelId] = firstUnseenGif;
		}

		GiphyData? DetermineFirstUnseenGif(ChannelSettingsModel channel)
		{
			if (channel.GifPosts is null)
				return _gifCache.GetFirstUnseenGif();

			var seenGifIds = channel.GifPosts.Select(s => s.GiphyDataId).ToList();

			return _gifCache.GetFirstUnseenGif(seenGifIds);
		}
	}

	public bool HasStagedGiphyData(ulong channelId) => _channelGifPostStage.ContainsKey(channelId);

	public GiphyData GetStagedGiphyData(ulong channelId) => _channelGifPostStage[channelId];

	public bool TryGetGiphyData(ulong channelId, out GiphyData? giphyData) => _channelGifPostStage.TryGetValue(channelId, out giphyData);
}