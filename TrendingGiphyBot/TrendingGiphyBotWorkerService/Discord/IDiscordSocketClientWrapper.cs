using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientWrapper
{
    event Func<SocketMessageComponent, Task> ButtonExecuted;
    event Func<SocketInteraction, Task> InteractionCreated;
    event Func<SocketGuild, Task> JoinedGuild;
    event Func<SocketGuild, Task> LeftGuild;
    event Func<LogMessage, Task> Log;
    event Func<SocketModal, Task> ModalSubmitted;
    event Func<Task> Ready;
    event Func<SocketMessageComponent, Task> SelectMenuExecuted;

    Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    Task LogoutAsync();
    Task StartAsync();
}
