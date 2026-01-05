using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientHandler
{
    Task JoinedGuild(SocketGuild arg);
    Task LeftGuild(SocketGuild arg);
    Task Log(LogMessage logMessage);
}