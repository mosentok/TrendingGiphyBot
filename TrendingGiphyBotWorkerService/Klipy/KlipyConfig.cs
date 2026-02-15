using TrendingGiphyBotWorkerService.Klipy.Staging;

namespace TrendingGiphyBotWorkerService.Klipy;

public record KlipyConfig(string BaseAddress, string CustomerId, KlipyStagingConfig Staging);
