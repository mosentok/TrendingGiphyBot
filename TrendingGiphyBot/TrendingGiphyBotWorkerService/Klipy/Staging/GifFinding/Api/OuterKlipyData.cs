using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

public class OuterKlipyData
{
    [JsonPropertyName("current_page")]
    public required int CurrentPage { get; set; }
    public required KlipyData[] Data { get; set; }

    [JsonPropertyName("per_page")]
    public required int PerPage { get; set; }

    [JsonPropertyName("has_next")]
    public required bool HasNext { get; set; }
}
