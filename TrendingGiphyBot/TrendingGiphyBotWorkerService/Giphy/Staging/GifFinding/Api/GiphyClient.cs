using Microsoft.Extensions.Options;
using System.Text.Json;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class GiphyClient(HttpClient _httpClient, IOptions<AppConfig> _appConfig) : IGiphyClient
{
    public async Task<GiphyResponse> GetTrendingGifsAsync(int offset = 0, int limit = 1000, string rating = "pg", string bundle = "clips_grid_picker", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_appConfig.Value.Giphy.ApiKey}&offset={offset}&limit={limit}&rating={rating}&bundle={bundle}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.GiphyResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<GiphyResponse> SearchAsync(string searchTerms, int offset = 0, int limit = 50, string rating = "pg", string lang ="en", string bundle = "clips_grid_picker", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_appConfig.Value.Giphy.ApiKey}&q={searchTerms}&offset={offset}&limit={limit}&rating={rating}&lang={lang}&bundle={bundle}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.GiphyResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<RandomResponse> GetRandomGifAsync(string rating = "pg", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/random?api_key={_appConfig.Value.Giphy.ApiKey}&rating={rating}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, GiphySourceGenerationContext.Default.RandomResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }
}
