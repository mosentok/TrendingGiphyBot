
namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

public interface IKlipyClient
{
    Task<KlipyResponse> GetRandomGifsAsync(int page = 1, int perPage = 50, string locale = "", string contentFilter = "high", string formatFilter = "gif", CancellationToken cancellationToken = default);
    Task<KlipyResponse> SearchGifsAsync(string query, int page = 1, int perPage = 50, string locale = "", string contentFilter = "high", string formatFilter = "gif", CancellationToken cancellationToken = default);
    Task<KlipyResponse> GetTrendingGifsAsync(int page = 1, int perPage = 50, string locale = "", string formatFilter = "gif", CancellationToken cancellationToken = default);
}