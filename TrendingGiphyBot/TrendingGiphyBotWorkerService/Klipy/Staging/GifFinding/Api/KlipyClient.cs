using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

public class KlipyClient(HttpClient _httpClient, IOptions<AppConfig> _appConfig) : IKlipyClient
{
    public async Task<KlipyResponse> GetTrendingGifsAsync(int page = 1, int perPage = 50, string locale = "", string formatFilter = "gif", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"trending?page={page}&per_page={perPage}&customer_id={_appConfig.Value.Klipy.CustomerId}&locale={locale}&format_filter={formatFilter}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, KlipySourceGenerationContext.Default.KlipyResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<KlipyResponse> SearchGifsAsync(string query, int page = 1, int perPage = 50, string locale = "", string contentFilter = "high", string formatFilter = "gif", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"search?page={page}&per_page={perPage}&q={query}&customer_id={_appConfig.Value.Klipy.CustomerId}&locale={locale}&format_filter={formatFilter}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, KlipySourceGenerationContext.Default.KlipyResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }

    public async Task<KlipyResponse> GetRandomGifsAsync(int page = 1, int perPage = 50, string locale = "", string contentFilter = "high", string formatFilter = "gif", CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync($"search?page={page}&per_page={perPage}&customer_id={_appConfig.Value.Klipy.CustomerId}&locale={locale}&content_filter={contentFilter}&format_filter={formatFilter}", cancellationToken);
        var stream = await result.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, KlipySourceGenerationContext.Default.KlipyResponse, cancellationToken) ?? throw new ThisShouldBeImpossibleException();
    }
}
