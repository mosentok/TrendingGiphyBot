using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record GiphyConfig(string ApiKey, string BaseAddress, StagingConfig Staging);
