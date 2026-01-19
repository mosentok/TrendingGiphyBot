using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class Images
{
    [JsonPropertyName("original")]
	public required ImageDetails Original { get; set; }

    [JsonPropertyName("fixed_width")]
	public required ImageDetails FixedWidth { get; set; }
}
