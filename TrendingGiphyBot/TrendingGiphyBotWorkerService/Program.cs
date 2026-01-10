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
var assembly = typeof(TgbSlashInteractionModule).Assembly;

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

var discordSocketConfig = new DiscordSocketConfig
{
	GatewayIntents =
		GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks |
		GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages |
		GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls |
		GatewayIntents.DirectMessagePolls
};

var discordSocketClient = new DiscordSocketClient(discordSocketConfig);
var discordWorkerConfig = new DiscordWorkerConfig(discordToken);
var discordSocketClientHandlerConfig = new DiscordSocketClientHandlerConfig(playingGame, guildToRegisterCommands, assembly);
var giphyConfig = new GiphyConfig(maxPageCount, timeSpanBetweenRefreshes);
var gifCache = new GifCache([], _maxCount: 288);
var channelSettingsMessageComponentFactory = new ChannelSettingsMessageComponentFactory(_minutes: [5, 10, 15, 30], _hours: [1, 2, 3, 4, 6, 8, 12, 24]);

//TODO map appsettings log level to discord.net's here
var interactionService = new InteractionService(discordSocketClient.Rest, new() { UseCompiledLambda = true, LogLevel = LogSeverity.Verbose });

builder.Services
	.AddHostedService<DiscordInteractionWorker>()
	.AddHostedService<GiphyCacheWorker>()
	.AddLogging(builder => builder.AddConsole())
	.AddDbContext<ITrendingGiphyBotDbContext, TrendingGiphyBotDbContext>(builder =>
	{
		var databasePath = Path.Combine(currentDirectory, "app.db");

		builder.UseSqlite($"Data Source={databasePath}");
	})
	.AddSingleton(discordSocketClient)
	.AddSingleton(discordWorkerConfig)
	.AddSingleton(discordSocketClientHandlerConfig)
	.AddSingleton(giphyConfig)
	.AddSingleton(interactionService)
	.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
	.AddSingleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>))
	.AddSingleton<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
	.AddSingleton<IGifCache>(gifCache)
	.AddSingleton<IChannelSettingsMessageComponentFactory>(channelSettingsMessageComponentFactory)
	.AddSingleton<IDiscordSocketClientWrapper, DiscordSocketClientWrapper>()
	.AddHttpClient<IGiphyClient, GiphyClient>(httpClient =>
	{
		//TODO move base address to config
		httpClient.BaseAddress = new("https://api.giphy.com/v1/");

		return new(httpClient, giphyApiKey);
	})
	.AddStandardResilienceHandler();

var host = builder.Build();

await host.RunAsync();