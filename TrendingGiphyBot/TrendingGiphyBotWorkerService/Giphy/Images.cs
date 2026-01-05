using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class Images
{
    [JsonPropertyName("original")]
    public ImageDetails Original { get; set; }

    [JsonPropertyName("fixed_width")]
    public ImageDetails FixedWidth { get; set; }
}
