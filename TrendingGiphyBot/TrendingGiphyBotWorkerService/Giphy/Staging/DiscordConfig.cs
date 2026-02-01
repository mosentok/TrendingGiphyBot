using Discord;
using TrendingGiphyBotWorkerService.Discord;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record DiscordConfig(LogSeverity LogSeverity, string Token, DiscordSocketClientHandlerConfig SocketClientHandler);
