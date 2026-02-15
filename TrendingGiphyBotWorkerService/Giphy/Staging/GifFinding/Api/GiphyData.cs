using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class GiphyData
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    // some results have 0000's for their TrendingDateTime
    [JsonPropertyName("trending_datetime")]
    public required string TrendingDatetime { get; set; }
}
