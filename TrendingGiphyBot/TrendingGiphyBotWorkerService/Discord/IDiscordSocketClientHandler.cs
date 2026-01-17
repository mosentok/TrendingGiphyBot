using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientHandler
{
    Task OnReadyAsync();
    Task OnInteractionCreatedAsync(SocketInteraction socketInteraction);
    Task OnComponentExecutedAsync(SocketMessageComponent interaction);
    Task OnModalSubmittedAsync(SocketModal arg);
    Task OnJoinedGuildAsync(SocketGuild arg);
    Task OnLeftGuildAsync(SocketGuild arg);
    Task OnLogAsync(LogMessage logMessage);
}