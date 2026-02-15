using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

[JsonSerializable(typeof(KlipyResponse))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class KlipySourceGenerationContext : JsonSerializerContext { }