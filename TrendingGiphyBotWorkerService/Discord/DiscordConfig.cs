using Discord;

namespace TrendingGiphyBotWorkerService.Discord;

public record DiscordConfig(LogSeverity LogSeverity, string Token, DiscordSocketClientHandlerConfig SocketClientHandler);
