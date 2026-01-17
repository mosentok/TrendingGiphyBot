using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordSocketClientWrapper(DiscordSocketClient _discordSocketClient) : IDiscordSocketClientWrapper
{
	public event Func<SocketMessageComponent, Task> ButtonExecuted
	{
		add => _discordSocketClient.ButtonExecuted += value;
		remove => _discordSocketClient.ButtonExecuted -= value;
	}

	public event Func<SocketInteraction, Task> InteractionCreated
	{
		add => _discordSocketClient.InteractionCreated += value;
		remove => _discordSocketClient.InteractionCreated -= value;
	}

	public event Func<SocketGuild, Task> JoinedGuild
	{
		add => _discordSocketClient.JoinedGuild += value;
		remove => _discordSocketClient.JoinedGuild -= value;
	}

	public event Func<SocketGuild, Task> LeftGuild
	{
		add => _discordSocketClient.LeftGuild += value;
		remove => _discordSocketClient.LeftGuild -= value;
	}

	public event Func<LogMessage, Task> Log
	{
		add => _discordSocketClient.Log += value;
		remove => _discordSocketClient.Log -= value;
	}

	public event Func<SocketModal, Task> ModalSubmitted
	{
		add => _discordSocketClient.ModalSubmitted += value;
		remove => _discordSocketClient.ModalSubmitted -= value;
	}

	public event Func<Task> Ready
	{
		add => _discordSocketClient.Ready += value;
		remove => _discordSocketClient.Ready -= value;
	}

	public event Func<SocketMessageComponent, Task> SelectMenuExecuted
	{
		add => _discordSocketClient.SelectMenuExecuted += value;
		remove => _discordSocketClient.SelectMenuExecuted -= value;
	}

	public Task LoginAsync(TokenType tokenType, string token, bool validateToken = true) =>
		_discordSocketClient.LoginAsync(tokenType, token, validateToken);

	public Task LogoutAsync() => _discordSocketClient.LogoutAsync();

	public Task StartAsync() => _discordSocketClient.StartAsync();

	public ValueTask<IChannel> GetChannelAsync(ulong channelId) => _discordSocketClient.GetChannelAsync(channelId);
}
