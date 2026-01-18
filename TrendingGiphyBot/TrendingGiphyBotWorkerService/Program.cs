using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

var builder = Host.CreateApplicationBuilder(args);

var currentDirectory = Directory.GetCurrentDirectory();
var databasePath = Path.Combine(currentDirectory, "app.db");
var connectionString = $"Data Source={databasePath}";
var assembly = typeof(TgbSlashInteractionModule).Assembly;

builder.Configuration
	.SetBasePath(currentDirectory)
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings.Development.json", optional: true)
	.AddEnvironmentVariables("Tgb__");

var discordToken = builder.Configuration.GetRequiredConfiguration("DiscordToken");
var giphyApiKey = builder.Configuration.GetRequiredConfiguration("GiphyApiKey");
var maxPageCount = builder.Configuration.GetRequiredConfiguration<int>("MaxPageCount");
var maxGiphyCacheLoops = builder.Configuration.GetRequiredConfiguration<int>("MaxGiphyCacheLoops");
var playingGame = builder.Configuration.GetRequiredConfiguration("PlayingGame");
var guildToRegisterCommands = builder.Configuration.GetOptionalConfiguration<ulong?>("RegisterCommandsToGuild");
var timeSpanBetweenCacheRefreshes = builder.Configuration.GetRequiredConfiguration<TimeSpan>("TimeSpanBetweenCacheRefreshes");
var timeSpanBetweenStageRefreshes = builder.Configuration.GetRequiredConfiguration<TimeSpan>("TimeSpanBetweenStageRefreshes");
var giphyBaseAddress = builder.Configuration.GetRequiredConfiguration("GiphyBaseAddress");
var discordLogLevel = builder.Configuration.GetRequiredConfiguration<LogSeverity>("DiscordLogLevel");

var discordSocketConfig = new DiscordSocketConfig
{
	GatewayIntents =
		GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks |
		GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages |
		GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls |
		GatewayIntents.DirectMessagePolls
};

var minutes = new[] { 5, 10, 15, 30 };
var hours = new[] { 1, 2, 3, 4, 6, 8, 12, 24 };
var intervalConfig = new IntervalConfig(minutes, hours);
var discordSocketClient = new DiscordSocketClient(discordSocketConfig);
var discordWorkerConfig = new DiscordWorkerConfig(discordToken);
var discordSocketClientHandlerConfig = new DiscordSocketClientHandlerConfig(playingGame, guildToRegisterCommands, assembly);
var gifCacheConfig = new GifCacheConfig([], 1_000);
var giphyCacheWorkerConfig = new GiphyCacheWorkerConfig(maxPageCount, timeSpanBetweenCacheRefreshes, maxGiphyCacheLoops);
var gifStagingWorkerConfig = new GifStagingWorkerConfig(timeSpanBetweenStageRefreshes);

var interactionService = new InteractionService(discordSocketClient.Rest, new() { UseCompiledLambda = true, LogLevel = discordLogLevel });

builder.Services
	.AddHostedService<DiscordInteractionWorker>()
	.AddHostedService<DiscordPostingWorker>()
	.AddHostedService<GiphyCacheWorker>()
	.AddHostedService<IntervalSeederWorker>()
	.AddLogging(builder => builder.AddConsole())
	.AddDbContext<ITrendingGiphyBotDbContext, TrendingGiphyBotDbContext>(builder => builder.UseSqlite(connectionString))
	.AddSingleton(discordSocketClient)
	.AddSingleton(discordSocketClientHandlerConfig)
	.AddSingleton(discordWorkerConfig)
	.AddSingleton(gifCacheConfig)
	.AddSingleton(giphyCacheWorkerConfig)
	.AddSingleton(gifStagingWorkerConfig)
	.AddSingleton(interactionService)
	.AddSingleton(intervalConfig)
	.AddSingleton(TimeProvider.System)
	.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
	.AddSingleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>))
	.AddSingleton<IChannelSettingsMessageComponentFactory, ChannelSettingsMessageComponentFactory>()
	.AddSingleton<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
	.AddSingleton<IDiscordSocketClientWrapper, DiscordSocketClientWrapper>()
	.AddSingleton<IGifCache, GifCache>()
	.AddSingleton<IGifPostStage, GifPostStage>()
	.AddHttpClient<IGiphyClient, GiphyClient>(httpClient =>
	{
		httpClient.BaseAddress = new(giphyBaseAddress);

		return new(httpClient, giphyApiKey);
	})
	.AddStandardResilienceHandler();

var host = builder.Build();

await host.RunAsync();