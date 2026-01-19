using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public class GifPostStage(IServiceScopeFactory _serviceScopeFactory, IGifCache _gifCache) : IGifPostStage
{
	readonly Dictionary<ulong, GiphyData> _channelGifPostStage = [];

	public IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage() => _channelGifPostStage.ToImmutableDictionary();

	public void Evict(ulong channelId) => _channelGifPostStage.Remove(channelId);

	public async Task RefreshAsync()
	{
		using var scope = _serviceScopeFactory.CreateScope();

		var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

		var activeChannels = await trendingGiphyBotDbContext.ChannelSettings.Where(s => s.HowOften != null).ToListAsync();

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
}