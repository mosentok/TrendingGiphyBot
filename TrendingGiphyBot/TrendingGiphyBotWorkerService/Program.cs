using Cronos;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics.CodeAnalysis;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Api.Paging;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.Caching;
using TrendingGiphyBotWorkerService.Interactions;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;
using TrendingGiphyBotWorkerService.Paging;
using TrendingGiphyBotWorkerService.Utc;

[assembly: SuppressMessage("Roslynator", "RCS1001:Add braces (when expression spans over multiple lines)", Justification = "Less is more.")]

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
var maxCacheLoops = builder.Configuration.GetRequiredConfiguration<int>("MaxCacheLoops");
var playingGame = builder.Configuration.GetRequiredConfiguration("PlayingGame");
var guildToRegisterCommands = builder.Configuration.GetOptionalConfiguration<ulong?>("RegisterCommandsToGuild");
var timeSpanBetweenCacheRefreshes = builder.Configuration.GetRequiredConfiguration<TimeSpan>("TimeSpanBetweenCacheRefreshes");
var timeSpanBetweenStageRefreshes = builder.Configuration.GetRequiredConfiguration<TimeSpan>("TimeSpanBetweenStageRefreshes");
var giphyBaseAddress = builder.Configuration.GetRequiredConfiguration("GiphyBaseAddress");
var discordLogLevel = builder.Configuration.GetRequiredConfiguration<LogSeverity>("DiscordLogLevel");
var maxRandomGifAttempts = builder.Configuration.GetRequiredConfiguration<int>("MaxRandomGifAttempts");
var enableTrendingGifs = builder.Configuration.GetRequiredConfiguration<bool>("EnableTrendingGifs");
var enableSearchGifs = builder.Configuration.GetRequiredConfiguration<bool>("EnableSearchGifs");
var enableRandomGifs = builder.Configuration.GetRequiredConfiguration<bool>("EnableRandomGifs");

var discordSocketConfig = new DiscordSocketConfig
{
    GatewayIntents =
        GatewayIntents.Guilds | GatewayIntents.GuildBans | GatewayIntents.GuildEmojis | GatewayIntents.GuildIntegrations | GatewayIntents.GuildWebhooks |
        GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages |
        GatewayIntents.DirectMessageReactions | GatewayIntents.DirectMessageTyping | GatewayIntents.AutoModerationConfiguration | GatewayIntents.AutoModerationActionExecution | GatewayIntents.GuildMessagePolls |
        GatewayIntents.DirectMessagePolls,
    UseInteractionSnowflakeDate = false
};

await using var discordSocketClient = new DiscordSocketClient(discordSocketConfig);

var minutes = new[] { 5, 10, 15, 30 };
var hours = new[] { 1, 2, 3, 4, 6, 8, 12, 24 };

var intervalConfig = new IntervalConfig(minutes, hours);

var every5Minutes = CronExpression.Parse("*/5 * * * *");

var delayerConfig = new DelayerConfig(every5Minutes);
var discordSocketClientHandlerConfig = new DiscordSocketClientHandlerConfig(playingGame, guildToRegisterCommands, assembly);
var gifCacheConfig = new GifCacheConfig(1_000);
var pagerConfig = new PagerConfig(maxCacheLoops);
var giphyCacheWorkerConfig = new GiphyCacheWorkerConfig(timeSpanBetweenCacheRefreshes);
var gifStagingWorkerConfig = new GifStagingWorkerConfig(timeSpanBetweenStageRefreshes);
var gifPostStageConfig = new GifPostStageConfig(maxRandomGifAttempts, enableTrendingGifs, enableSearchGifs, enableRandomGifs);
var giphyClientConfig = new GiphyClientConfig(giphyApiKey);
var interactionService = new InteractionService(discordSocketClient.Rest, new() { UseCompiledLambda = true, LogLevel = discordLogLevel, DefaultRunMode = RunMode.Async });
var logPath = Path.Combine(currentDirectory, "logs", "log.log");

const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u4}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

builder.Services
    .AddHostedService<DiscordPostingWorker>()
    .AddHostedService<GifStagingWorker>()
    .AddHostedService<GiphyCacheWorker>()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("TrendingGiphyBotWorkerService", LogEventLevel.Verbose)
            .MinimumLevel.Override("Discord", LogEventLevel.Verbose)
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: outputTemplate,
                theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                logPath,
                outputTemplate: outputTemplate,
                rollingInterval: RollingInterval.Day
            )
            .CreateLogger();

        loggingBuilder.AddSerilog(logger);
    })
    .AddDbContext<ITrendingGiphyBotDbContext, TrendingGiphyBotDbContext>(dbContextOptionsBuilder =>
        dbContextOptionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlite(connectionString))
    .AddSingleton(delayerConfig)
    .AddSingleton(discordSocketClient)
    .AddSingleton(discordSocketClientHandlerConfig)
    .AddSingleton(gifCacheConfig)
    .AddSingleton(gifPostStageConfig)
    .AddSingleton(gifStagingWorkerConfig)
    .AddSingleton(giphyCacheWorkerConfig)
    .AddSingleton(giphyClientConfig)
    .AddSingleton(interactionService)
    .AddSingleton(intervalConfig)
    .AddSingleton(pagerConfig)
    .AddSingleton(TimeProvider.System)
    .AddSingleton<IChannelSettingsFilter, ChannelSettingsFilter>()
    .AddSingleton<IChannelSettingsFinder, ChannelSettingsFinder>()
    .AddSingleton<IChannelSettingsMessageComponentFactory, ChannelSettingsMessageComponentFactory>()
    .AddSingleton<IDelayer, Delayer>()
    .AddSingleton<IDiscordChannelGifPoster, DiscordChannelGifPoster>()
    .AddSingleton<IDiscordSocketClientHandler, DiscordSocketClientHandler>()
    .AddSingleton<IDiscordSocketClientWrapper, DiscordSocketClientWrapper>()
    .AddSingleton<IGifFinder, GifFinder>()
    .AddSingleton<IGifPostingBehaviorHelper, GifPostingBehaviorHelper>()
    .AddSingleton<IGifPostingBehaviorSeeder, GifPostingBehaviorSeeder>()
    .AddSingleton<IGifPostStage, GifPostStage>()
    .AddSingleton<IGiphyDataListHelper, GiphyDataListHelper>()
    .AddSingleton<IGiphySearchCache, GiphySearchCache>()
    .AddSingleton<IGiphySearchPager, GiphySearchPager>()
    .AddSingleton<IGiphyTrendingCache, GiphyTrendingCache>()
    .AddSingleton<IGiphyTrendingPager, GiphyTrendingPager>()
    .AddSingleton<IIntervalSeeder, IntervalSeeder>()
    .AddSingleton<IPager, Pager>()
    .AddSingleton<IUtcOffsetParser, UtcOffsetParser>()
    .AddHttpClient<IGiphyClient, GiphyClient>(s => s.BaseAddress = new(giphyBaseAddress))
    .AddStandardResilienceHandler();

var host = builder.Build();

var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var intervalSeeder = host.Services.GetRequiredService<IIntervalSeeder>();
var giphyTrendingCache = host.Services.GetRequiredService<IGiphyTrendingCache>();
var gifPostStage = host.Services.GetRequiredService<IGifPostStage>();
var gifPostingBehaviorSeeder = host.Services.GetRequiredService<IGifPostingBehaviorSeeder>();

discordSocketClient.ButtonExecuted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.InteractionCreated += discordSocketClientHandler.OnInteractionCreatedAsync;
discordSocketClient.JoinedGuild += discordSocketClientHandler.OnJoinedGuildAsync;
discordSocketClient.LeftGuild += discordSocketClientHandler.OnLeftGuildAsync;
discordSocketClient.Log += discordSocketClientHandler.OnLogAsync;
discordSocketClient.ModalSubmitted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.Ready += discordSocketClientHandler.OnReadyAsync;
discordSocketClient.SelectMenuExecuted += discordSocketClientHandler.OnSocketInteractionAsync;

interactionService.Log += discordSocketClientHandler.OnLogAsync;

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInitializing();

    if (builder.Environment.IsDevelopment())
    {
        var debugView = Environment.NewLine + builder.Configuration.GetDebugView().TrimEnd();

        logger.LogDebugView(debugView);
    }

    await gifPostingBehaviorSeeder.SeedGifPostingBehaviorsAsync();
    await intervalSeeder.SeedIntervalsAsync();
    await giphyTrendingCache.RefreshTrendingGifsAsync();
    await gifPostStage.RefreshAsync();

    await discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
    await discordSocketClient.StartAsync();

    logger.LogInitialized();

    await host.RunAsync();
}
catch (Exception exception)
{
    logger.LogTopLevelException(exception);
}
finally
{
    await discordSocketClient.LogoutAsync();
}