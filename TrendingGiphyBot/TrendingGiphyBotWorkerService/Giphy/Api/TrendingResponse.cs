using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class TrendingResponse
{
    [JsonPropertyName("data")]
    public required List<GiphyData> Data { get; set; }

    [JsonPropertyName("meta")]
    public required Meta Meta { get; set; }

    [JsonPropertyName("pagination")]
    public required Pagination Pagination { get; set; }
}
