using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Discord.GifPosting;

namespace TrendingGiphyBotWorkerService.Discord
{
    public interface IKlipyDataChannelPoster
    {
        Task PostKlipyGifsAsync(IImmutableDictionary<ulong, KlipyGifPostSelection> selections, CancellationToken stoppingToken);
    }
}

