using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService;

var builder = Host.CreateApplicationBuilder(args);

var currentDirectory = Directory.GetCurrentDirectory();

builder.Configuration
	.SetBasePath(currentDirectory)
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Development.json", optional: true)
	.AddEnvironmentVariables();

builder.Services
	.AddHostedService<Worker>()
	.AddLogging(builder => builder.AddConsole())
	.AddDbContext<ITrendingGiphyBotContext, TrendingGiphyBotContext>(builder =>
	{
		var databasePath = Path.Combine(currentDirectory, "app.db");

		builder.UseSqlite($"Data Source={databasePath}");
	})
	.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
	.AddSingleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>))
	.AddSingleton<IChannelSettingsMessageComponentFactory>(_ => new ChannelSettingsMessageComponentFactory(_minutes: [5, 10, 15, 30], _hours: [1, 2, 3, 4, 6, 8, 12, 24]))
	.AddSingleton<DiscordSocketClient>()
	.AddSingleton<InteractionService>(services =>
	{
		var discordSocketClient = services.GetRequiredService<DiscordSocketClient>();

		return new(discordSocketClient.Rest);
	})
	.AddHttpClient<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
	.AddStandardResilienceHandler()
;

var host = builder.Build();
var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var discordSocketClient = host.Services.GetRequiredService<DiscordSocketClient>();
var interactionService = host.Services.GetRequiredService<InteractionService>();
var discordToken = builder.Configuration.GetRequiredConfiguration("DiscordToken");
var playingGame = builder.Configuration.GetRequiredConfiguration("PlayingGame");
var guildToRegisterCommands = builder.Configuration.GetSection("RegisterCommandsToGuild").Get<ulong?>();
var assembly = typeof(TgbSlashInteractionModule).Assembly;

discordSocketClient.Log += discordSocketClientHandler.Log;
discordSocketClient.JoinedGuild += discordSocketClientHandler.JoinedGuild;
discordSocketClient.LeftGuild += discordSocketClientHandler.LeftGuild;
discordSocketClient.Ready += DiscordSocketClient_Ready;
interactionService.Log += discordSocketClientHandler.Log;

await discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
await discordSocketClient.StartAsync();
await host.RunAsync();

async Task DiscordSocketClient_Ready()
{
	await discordSocketClient.SetGameAsync(playingGame);
	await interactionService.AddModulesAsync(assembly, host.Services);

	if (guildToRegisterCommands is not null)
		await interactionService.RegisterCommandsToGuildAsync(guildToRegisterCommands.Value);
	else
		await interactionService.RegisterCommandsGloballyAsync();

	discordSocketClient.InteractionCreated += async interaction =>
	{
		if (interaction.Type is not InteractionType.ApplicationCommand)
			return;

		var socketInteractionContext = new SocketInteractionContext(discordSocketClient, interaction);

		await interactionService.ExecuteCommandAsync(socketInteractionContext, host.Services);
	};

	discordSocketClient.ModalSubmitted += DiscordSocketClient_ModalSubmitted;

	discordSocketClient.SelectMenuExecuted += HandleInteraction;

	discordSocketClient.ButtonExecuted += HandleInteraction;

	async Task HandleInteraction(SocketMessageComponent interaction)
	{
		var socketInteractionContext = new SocketInteractionContext<SocketMessageComponent>(discordSocketClient, interaction);

		await interactionService.ExecuteCommandAsync(socketInteractionContext, host.Services);
	}

	async Task DiscordSocketClient_ModalSubmitted(SocketModal arg)
	{
		var socketInteractionContext = new SocketInteractionContext<SocketModal>(discordSocketClient, arg);

		await interactionService.ExecuteCommandAsync(socketInteractionContext, host.Services);
	}
}