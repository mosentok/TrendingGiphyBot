using Cronos;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;
using TrendingGiphyBotWorkerService.Interactions;
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
var maxRandomGifAttempts = builder.Configuration.GetRequiredConfiguration<int>("MaxRandomGifAttempts");

var discordSocketConfig = new DiscordSocketConfig
{
	GatewayIntents =
		GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks |
		GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages |
		GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls |
		GatewayIntents.DirectMessagePolls,
    UseInteractionSnowflakeDate = false
};

var minutes = new[] { 5, 10, 15, 30 };
var hours = new[] { 1, 2, 3, 4, 6, 8, 12, 24 };

var intervalConfig = new IntervalConfig(minutes, hours);

var every5Minutes = CronExpression.Parse("*/5 * * * *");

var delayerConfig = new DelayerConfig(every5Minutes);
var discordSocketClient = new DiscordSocketClient(discordSocketConfig);
var discordSocketClientHandlerConfig = new DiscordSocketClientHandlerConfig(playingGame, guildToRegisterCommands, assembly);
var gifCacheConfig = new GifCacheConfig([], 1_000, maxPageCount, maxGiphyCacheLoops);
var giphyCacheWorkerConfig = new GiphyCacheWorkerConfig(timeSpanBetweenCacheRefreshes);
var gifStagingWorkerConfig = new GifStagingWorkerConfig(timeSpanBetweenStageRefreshes);
var gifPostStageConfig = new GifPostStageConfig(maxRandomGifAttempts);
var giphyClientConfig = new GiphyClientConfig(giphyApiKey);
var interactionService = new InteractionService(discordSocketClient.Rest, new() { UseCompiledLambda = true, LogLevel = discordLogLevel, DefaultRunMode = RunMode.Async });

builder.Services
	.AddHostedService<DiscordPostingWorker>()
	.AddHostedService<GifStagingWorker>()
	.AddHostedService<GiphyCacheWorker>()
	.AddLogging(builder => builder.AddConsole())
	.AddDbContext<ITrendingGiphyBotDbContext, TrendingGiphyBotDbContext>(builder =>
		builder
            .EnableSensitiveDataLogging()
            .UseSqlite(connectionString))
	.AddSingleton(delayerConfig)
	.AddSingleton(discordSocketClient)
	.AddSingleton(discordSocketClientHandlerConfig)
	.AddSingleton(gifCacheConfig)
	.AddSingleton(gifPostStageConfig )
    .AddSingleton(gifStagingWorkerConfig)
	.AddSingleton(giphyCacheWorkerConfig)
	.AddSingleton(giphyClientConfig)
	.AddSingleton(interactionService)
	.AddSingleton(intervalConfig)
	.AddSingleton(TimeProvider.System)
	.AddSingleton<IChannelSettingsMessageComponentFactory, ChannelSettingsMessageComponentFactory>()
	.AddSingleton<IDelayer, Delayer>()
	.AddSingleton<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
	.AddSingleton<IDiscordSocketClientWrapper, DiscordSocketClientWrapper>()
    .AddSingleton<IGifCache, GifCache>()
	.AddSingleton<IGifPostingBehaviorHelper, GifPostingBehaviorHelper>()
	.AddSingleton<IGifPostingBehaviorSeeder, GifPostingBehaviorSeeder>()
    .AddSingleton<IGifPostStage, GifPostStage>()
	.AddSingleton<IIntervalSeeder, IntervalSeeder>()
    .AddHttpClient<IGiphyClient, GiphyClient>(s => s.BaseAddress = new(giphyBaseAddress))
	.AddStandardResilienceHandler();

var host = builder.Build();

var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var intervalSeeder = host.Services.GetRequiredService<IIntervalSeeder>();
var gifCache = host.Services.GetRequiredService<IGifCache>();
var gifPostStage = host.Services.GetRequiredService<IGifPostStage>();
var gifPostingBehaviorSeeder = host.Services.GetRequiredService<IGifPostingBehaviorSeeder>();

discordSocketClient.ButtonExecuted += discordSocketClientHandler.OnComponentExecutedAsync;
discordSocketClient.InteractionCreated += discordSocketClientHandler.OnInteractionCreatedAsync;
discordSocketClient.JoinedGuild += discordSocketClientHandler.OnJoinedGuildAsync;
discordSocketClient.LeftGuild += discordSocketClientHandler.OnLeftGuildAsync;
discordSocketClient.Log += discordSocketClientHandler.OnLogAsync;
discordSocketClient.ModalSubmitted += discordSocketClientHandler.OnModalSubmittedAsync;
discordSocketClient.Ready += discordSocketClientHandler.OnReadyAsync;
discordSocketClient.SelectMenuExecuted += discordSocketClientHandler.OnComponentExecutedAsync;

interactionService.Log += discordSocketClientHandler.OnLogAsync;

try
{
	await gifPostingBehaviorSeeder.SeedGifPostingBehaviorsAsync();
    await intervalSeeder.SeedIntervalsAsync();
    await gifCache.RefreshAsync();
    await gifPostStage.RefreshAsync();

    await discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
	await discordSocketClient.StartAsync();

	await host.RunAsync();
}
catch (Exception exception)
{
	var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Top Level");

    logger.LogTopLevelException(exception);
}
finally
{
    await discordSocketClient.LogoutAsync();
	await discordSocketClient.DisposeAsync();
}