using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService;

public interface IDiscordSocketClientHandler
{
    Task JoinedGuild(SocketGuild arg);
    Task LeftGuild(SocketGuild arg);
    Task Log(LogMessage logMessage);
}