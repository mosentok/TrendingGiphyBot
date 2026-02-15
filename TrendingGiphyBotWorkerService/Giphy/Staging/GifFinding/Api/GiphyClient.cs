using Microsoft.Extensions.Options;
using System.Text.Json;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class GiphyClient(
    HttpClient _httpClient,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyClient
{
    public async Task<GiphyResponse> GetTrendingGifsAsync(
        string rating,
        int offset = 0,
        int limit = 1000,
        string bundle = "clips_grid_picker",
        CancellationToken cancellationToken = default)
    {
        var ratingParam = rating == "all" ? "" : $"&rating={rating}";
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_appConfig.CurrentValue.Giphy.ApiKey}&offset={offset}&limit={limit}{ratingParam}&bundle={bundle}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
        var deserialized = await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.GiphyResponse, cancellationToken);

        return deserialized ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<GiphyResponse> SearchAsync(
        string searchTerms,
        string rating,
        int offset = 0,
        int limit = 50,
        string lang = "en",
        string bundle = "clips_grid_picker",
        CancellationToken cancellationToken = default)
    {
        var ratingParam = rating == "all" ? "" : $"&rating={rating}";
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_appConfig.CurrentValue.Giphy.ApiKey}&q={searchTerms}&offset={offset}&limit={limit}{ratingParam}&lang={lang}&bundle={bundle}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
        var deserialized = await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.GiphyResponse, cancellationToken);

        return deserialized ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<RandomResponse> GetRandomGifAsync(string rating, CancellationToken cancellationToken = default)
    {
        var ratingParam = rating == "all" ? "" : $"&rating={rating}";
        var result = await _httpClient.GetAsync($"gifs/random?api_key={_appConfig.CurrentValue.Giphy.ApiKey}{ratingParam}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
        var deserialized = await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.RandomResponse, cancellationToken);

        return deserialized ?? throw new ThisShouldBeImpossibleException();
    }
}
