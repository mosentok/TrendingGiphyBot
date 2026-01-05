using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyResponse
{
    [JsonPropertyName("data")]
    public List<GiphyData> Data { get; set; }

    [JsonPropertyName("meta")]
    public Meta Meta { get; set; }

    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; }
}
