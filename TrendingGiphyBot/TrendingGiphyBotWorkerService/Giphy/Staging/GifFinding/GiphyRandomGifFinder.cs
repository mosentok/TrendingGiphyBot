using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public class GiphyRandomGifFinder
(
IGiphyClient _giphyClient,
IOptions<AppConfig> _appConfig
) : IGiphyRandomGifFinder
{
    public async Task<Maybe<GiphyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        if (!_appConfig.Value.Giphy.Staging.EnableRandomGifs)
            return new();

        var attempts = 0;

        do
        {
            var randomGif = await _giphyClient.GetRandomGifAsync(cancellationToken: cancellationToken);

            if (channel.GifPosts is null or { Count: 0 })
                return new(randomGif.Data);

            var seenGiphyDataIds = channel.GifPosts.Select(s => s.GiphyDataId).ToArray();
            var randomGifHasAlreadyBeenSeen = seenGiphyDataIds.Contains(randomGif.Data.Id);

            if (!randomGifHasAlreadyBeenSeen)
                return new(randomGif.Data);

            attempts++;
        } while (attempts < _appConfig.Value.Giphy.Staging.MaxRandomGifAttempts);

        return new();
    }
}
