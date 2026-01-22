using System.Text.Json;

namespace TrendingGiphyBotWorkerService.Giphy.Api;
public class GiphyClient(HttpClient _httpClient, GiphyClientConfig _giphyClientConfig) : IGiphyClient
{
    public async Task<TrendingResponse> GetTrendingGifsAsync(int? offset = 0, int? limit = 1000, string? rating = "pg", string? bundle = "clips_grid_picker", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_giphyClientConfig.GiphyApiKey}&offset={offset}&limit={limit}&rating={rating}&bundle={bundle}", cancellationToken);

        //TODO why do some results have 0000's for their GiphyData.TrendingDateTime?
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<TrendingResponse>(stream, cancellationToken: cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<RandomResponse> GetRandomGifAsync(string? keyword = null, string? rating = "pg", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/random?api_key={_giphyClientConfig.GiphyApiKey}&tag={keyword}&rating={rating}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<RandomResponse>(stream, cancellationToken: cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }
}
