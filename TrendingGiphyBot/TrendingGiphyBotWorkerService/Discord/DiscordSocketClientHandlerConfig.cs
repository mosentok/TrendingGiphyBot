using System.Reflection;

namespace TrendingGiphyBotWorkerService.Discord
{
    public record DiscordSocketClientHandlerConfig(string PlayingGame, ulong? GuildToRegisterCommands, Assembly Assembly);
}
