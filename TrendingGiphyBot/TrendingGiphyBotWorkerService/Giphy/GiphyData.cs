using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyData
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("bitly_gif_url")]
    public required string BitlyGifUrl { get; set; }

    [JsonPropertyName("bitly_url")]
    public required string BitlyUrl { get; set; }

    [JsonPropertyName("embed_url")]
    public required string EmbedUrl { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("rating")]
    public required string Rating { get; set; }

    [JsonPropertyName("is_sticker")]
    public int IsSticker { get; set; }

    [JsonPropertyName("import_datetime")]
    public required string ImportDatetime { get; set; }

    [JsonPropertyName("trending_datetime")]
    public required string TrendingDatetime { get; set; }

    [JsonPropertyName("images")]
    public required Images Images { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("analytics_response_payload")]
    public required string AnalyticsResponsePayload { get; set; }

    [JsonPropertyName("alt_text")]
    public required string AltText { get; set; }
}
