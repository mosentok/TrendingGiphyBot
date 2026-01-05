using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class Meta
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("response_id")]
    public string ResponseId { get; set; }
}
