using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class ImageDetails
{
    [JsonPropertyName("height")]
    public required string Height { get; set; }

    [JsonPropertyName("width")]
    public required string Width { get; set; }

    [JsonPropertyName("size")]
    public required string Size { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("mp4_size")]
    public required string Mp4Size { get; set; }

    [JsonPropertyName("mp4")]
    public required string Mp4 { get; set; }

    [JsonPropertyName("webp_size")]
    public required string WebpSize { get; set; }

    [JsonPropertyName("webp")]
    public required string Webp { get; set; }

    [JsonPropertyName("frames")]
    public string? Frames { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
}
