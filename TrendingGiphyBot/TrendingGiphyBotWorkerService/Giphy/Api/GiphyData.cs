using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class GiphyData
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("trending_datetime")]
    public required string TrendingDatetime { get; set; }
}
