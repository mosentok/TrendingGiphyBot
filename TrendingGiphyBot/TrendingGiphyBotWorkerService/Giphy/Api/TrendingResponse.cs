using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class TrendingResponse
{
    [JsonPropertyName("data")]
    public required List<GiphyData> Data { get; set; }
}
