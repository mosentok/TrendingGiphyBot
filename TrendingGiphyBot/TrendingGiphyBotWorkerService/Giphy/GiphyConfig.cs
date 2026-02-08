using TrendingGiphyBotWorkerService.Giphy.Staging;

namespace TrendingGiphyBotWorkerService.Giphy;

public record GiphyConfig(string ApiKey, string BaseAddress, GiphyStagingConfig Staging);
