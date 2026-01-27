using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

[JsonSerializable(typeof(GiphyResponse))]
[JsonSerializable(typeof(RandomResponse))]
public partial class SourceGenerationContext : JsonSerializerContext { }