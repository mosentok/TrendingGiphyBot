using System.Text.Json.Serialization;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

[JsonSerializable(typeof(GiphyResponse))]
[JsonSerializable(typeof(RandomResponse))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class SourceGenerationContext : JsonSerializerContext { }