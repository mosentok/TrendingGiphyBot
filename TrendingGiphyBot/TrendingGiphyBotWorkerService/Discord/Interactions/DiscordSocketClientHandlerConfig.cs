using System.Reflection;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public record DiscordSocketClientHandlerConfig(string PlayingGame, ulong? GuildToRegisterCommands, Assembly Assembly);
