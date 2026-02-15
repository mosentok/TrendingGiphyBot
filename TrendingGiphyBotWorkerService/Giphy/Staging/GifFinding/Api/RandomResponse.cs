using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class RandomResponse
{
    [JsonPropertyName("data")]
    public required GiphyData Data { get; set; }
}
