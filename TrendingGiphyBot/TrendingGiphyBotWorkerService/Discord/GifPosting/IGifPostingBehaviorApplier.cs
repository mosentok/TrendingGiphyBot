using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public interface IGifPostingBehaviorApplier
{
	GifPostingSelections SelectGifsForPosting(
		IImmutableDictionary<ulong, GiphyData> stagedGiphyTrending,
		IImmutableDictionary<ulong, GiphyData> stagedGiphySearch,
		IImmutableDictionary<ulong, GiphyData> stagedGiphyRandom,
		IImmutableDictionary<ulong, KlipyData> stagedKlipyTrending,
		IImmutableDictionary<ulong, KlipyData> stagedKlipySearch,
		IImmutableDictionary<ulong, KlipyData> stagedKlipyRandom,
		List<ChannelSettingsModel> channelSettings
	);
}
