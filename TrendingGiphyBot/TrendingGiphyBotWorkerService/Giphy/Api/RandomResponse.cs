using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class RandomResponse
{
    [JsonPropertyName("data")]
    public required GiphyData Data { get; set; }
}
