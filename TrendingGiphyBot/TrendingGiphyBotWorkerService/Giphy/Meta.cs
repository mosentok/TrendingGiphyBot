using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class Meta
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("msg")]
    public required string Msg { get; set; }

    [JsonPropertyName("response_id")]
    public required string ResponseId { get; set; }
}
