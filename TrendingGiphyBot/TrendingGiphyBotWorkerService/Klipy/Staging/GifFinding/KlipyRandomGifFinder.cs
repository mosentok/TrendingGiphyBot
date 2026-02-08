using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyRandomGifFinder
(
    IKlipyClient _klipyClient,
    IOptions<AppConfig> _appConfig
) : IKlipyRandomGifFinder
{
    public async Task<Maybe<KlipyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        if (!_appConfig.Value.Klipy.Staging.EnableRandomGifs)
            return new();

        var randomGif = await _klipyClient.GetRandomGifsAsync(cancellationToken: cancellationToken);

        if (randomGif.Data.Data.Length == 0)
            return new();

        if (channel.KlipyPosts.Count == 0)
            return new(randomGif.Data.Data[0]);

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();

        var firstUnseen = randomGif.Data.Data.FirstOrDefault(s => seenKlipyDataIds.Contains(s.Id));

        return firstUnseen is not null
            ? new(firstUnseen)
            : new();
    }
}
