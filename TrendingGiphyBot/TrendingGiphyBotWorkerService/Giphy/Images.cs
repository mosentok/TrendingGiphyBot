using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class Images
{
    [JsonPropertyName("original")]
	public required ImageDetails Original { get; set; }

    [JsonPropertyName("fixed_width")]
	public required ImageDetails FixedWidth { get; set; }
}
