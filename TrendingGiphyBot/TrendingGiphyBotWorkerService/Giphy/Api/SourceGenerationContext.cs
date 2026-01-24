using System.Text.Json.Serialization;

namespace TrendingGiphyBotWorkerService.Giphy.Api;

[JsonSerializable(typeof(TrendingResponse))]
[JsonSerializable(typeof(RandomResponse))]
public partial class SourceGenerationContext : JsonSerializerContext { }