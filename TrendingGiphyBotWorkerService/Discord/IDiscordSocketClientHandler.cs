using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientHandler
{
    Task OnSocketInteractionAsync<T>(T interaction) where T : SocketInteraction;
    Task OnReadyAsync();
    Task OnInteractionCreatedAsync(SocketInteraction socketInteraction);
    Task OnJoinedGuildAsync(SocketGuild arg);
    Task OnLeftGuildAsync(SocketGuild arg);
    Task OnLogAsync(LogMessage logMessage);
}