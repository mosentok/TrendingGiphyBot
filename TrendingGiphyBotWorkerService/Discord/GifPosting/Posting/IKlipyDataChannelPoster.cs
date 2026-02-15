using System.Collections.Immutable;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

public interface IKlipyDataChannelPoster
{
    Task PostKlipyGifsAsync(IImmutableDictionary<ulong, KlipyGifPostSelection> selections, CancellationToken stoppingToken);
}