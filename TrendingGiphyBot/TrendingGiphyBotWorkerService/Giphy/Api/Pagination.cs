using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class Pagination
{
    public int Offset { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    public int Count { get; set; }
}