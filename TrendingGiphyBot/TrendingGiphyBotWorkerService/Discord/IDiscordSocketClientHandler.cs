using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientHandler
{
    Task OnReady();
    Task OnInteractionCreated(SocketInteraction socketInteraction);
    Task OnComponentExecuted(SocketMessageComponent interaction);
    Task OnModalSubmitted(SocketModal arg);
    Task OnJoinedGuild(SocketGuild arg);
    Task OnLeftGuild(SocketGuild arg);
    Task OnLog(LogMessage logMessage);
}