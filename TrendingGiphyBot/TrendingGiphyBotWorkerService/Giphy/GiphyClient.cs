using System.Text.Json;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyClient(HttpClient _httpClient, string _giphyApiKey) : IGiphyClient
{
    public async Task<GiphyResponse> GetTrendingGifsAsync(int? offset = 0, int? limit = 1000, string? rating = "pg", string? bundle = "clips_grid_picker", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"gifs/trending?api_key={_giphyApiKey}&offset={offset}&limit={limit}&rating={rating}&bundle={bundle}", cancellationToken);

        //TODO why do some results have 0000's for their GiphyData.TrendingDateTime?
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<GiphyResponse>(stream, cancellationToken: cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }
}
