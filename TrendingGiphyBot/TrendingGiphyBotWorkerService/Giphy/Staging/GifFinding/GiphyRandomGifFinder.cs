using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
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

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var attempts = 0;

        do
        {
            var randomGif = await _giphyClient.GetRandomGifAsync(cancellationToken: cancellationToken);

            if (channel.GiphyPosts is null or { Count: 0 })
                return new(randomGif.Data);

            var randomGifHasAlreadyBeenSeen = seenGiphyDataIds.Contains(randomGif.Data.Id);

            if (!randomGifHasAlreadyBeenSeen)
                return new(randomGif.Data);

            attempts++;
        } while (attempts < _appConfig.Value.Giphy.Staging.MaxRandomGifAttempts);

        return new();
    }
}
