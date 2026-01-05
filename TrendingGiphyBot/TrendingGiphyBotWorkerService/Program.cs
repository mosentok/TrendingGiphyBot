using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Logging;

var builder = Host.CreateApplicationBuilder(args);

var currentDirectory = Directory.GetCurrentDirectory();

builder.Configuration
	.SetBasePath(currentDirectory)
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Development.json", optional: true)
	.AddEnvironmentVariables("Tgb__");

var discordToken = builder.Configuration.GetRequiredConfiguration("DiscordToken");
var giphyApiKey = builder.Configuration.GetRequiredConfiguration("GiphyApiKey");
var maxPageCount = builder.Configuration.GetRequiredConfiguration<int>("MaxPageCount");
var playingGame = builder.Configuration.GetRequiredConfiguration("PlayingGame");
var guildToRegisterCommands = builder.Configuration.GetOptionalConfiguration<ulong?>("RegisterCommandsToGuild");
var timeSpanBetweenRefreshes = builder.Configuration.GetRequiredConfiguration<TimeSpan>("TimeSpanBetweenRefreshes");
var assembly = typeof(TgbSlashInteractionModule).Assembly;

builder.Services
	.AddHostedService<Worker>()
	.AddHostedService<GiphyCacheWorker>(services =>
	{
		var loggerWrapper = services.GetRequiredService<ILoggerWrapper<GiphyCacheWorker>>();
		var giphyClient = services.GetRequiredService<IGiphyClient>();
		var gifCache = services.GetRequiredService<IGifCache>();

		return new(loggerWrapper, giphyClient, gifCache, maxPageCount, timeSpanBetweenRefreshes);
	})
	.AddLogging(builder => builder.AddConsole())
	.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
	.AddSingleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>))
	.AddDbContext<ITrendingGiphyBotContext, TrendingGiphyBotContext>(builder =>
	{
		var databasePath = Path.Combine(currentDirectory, "app.db");

		builder.UseSqlite($"Data Source={databasePath}");
	})
	.AddSingleton<IGifCache>(_ => new GifCache([], _maxCount: 288))
	.AddSingleton<IChannelSettingsMessageComponentFactory>(_ => new ChannelSettingsMessageComponentFactory(_minutes: [5, 10, 15, 30], _hours: [1, 2, 3, 4, 6, 8, 12, 24]))
	.AddSingleton<DiscordSocketClient>(_ =>
	{
		var discordSocketConfig = new DiscordSocketConfig
		{
			GatewayIntents =
				GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks |
				GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages |
				GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls |
				GatewayIntents.DirectMessagePolls
		};

		return new(discordSocketConfig);
	})
	.AddSingleton<InteractionService>(services =>
	{
		var discordSocketClient = services.GetRequiredService<DiscordSocketClient>();

		//TODO map appsettings log level to discord.net's here
		return new(discordSocketClient.Rest, new() { UseCompiledLambda = true, LogLevel = LogSeverity.Verbose });
	})
	.AddHttpClient<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
	.AddStandardResilienceHandler()
	.Services
	.AddHttpClient<IGiphyClient, GiphyClient>(httpClient =>
	{
		httpClient.BaseAddress = new("https://api.giphy.com/v1/");

		return new(httpClient, giphyApiKey);
	})
	.AddStandardResilienceHandler();

var host = builder.Build();
var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var discordSocketClient = host.Services.GetRequiredService<DiscordSocketClient>();
var interactionService = host.Services.GetRequiredService<InteractionService>();

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