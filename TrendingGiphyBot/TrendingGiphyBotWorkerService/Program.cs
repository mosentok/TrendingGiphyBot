using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TrendingGiphyBotWorkerService;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Discord.GifPosting;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Logging;

[assembly: SuppressMessage("Roslynator", "RCS1001:Add braces (when expression spans over multiple lines)", Justification = "Less is more.")]

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

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

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.log");

const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u4}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

var appConfigSection = builder.Configuration.GetSection("Tgb");

builder.Services
    .Configure<AppConfig>(appConfigSection)
    .AddHostedService<DiscordPostingWorker>()
    .AddHostedService<GiphyDataStagingWorker>()
    .AddHostedService<GiphyTrendingCacheWorker>()
    .AddHostedService<GiphyRandomCacheWorker>()
    .AddHostedService<KlipyDataStagingWorker>()
    .AddHostedService<KlipyTrendingCacheWorker>()
    .AddHostedService<KlipyRandomCacheWorker>()
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
    {
        var databasePath = Path.Combine(AppContext.BaseDirectory, "app.db");

        dbContextOptionsBuilder
            .EnableSensitiveDataLogging()
            .UseSqlite($"Data Source={databasePath}");
    })
    .AddSingleton(discordSocketClient)
    .AddSingleton(services =>
    {
        var appConfig = services.GetRequiredService<IOptionsMonitor<AppConfig>>();

        return new InteractionService(discordSocketClient, new() { UseCompiledLambda = true, LogLevel = appConfig.CurrentValue.Discord.LogSeverity, DefaultRunMode = RunMode.Async });
    })
    .AddSingleton(TimeProvider.System)
    .AddTrendingGiphyBotWorkerService()
    .AddHttpClient<IGiphyClient, GiphyClient>((services, httpClilent) =>
    {
        var appConfig = services.GetRequiredService<IOptionsMonitor<AppConfig>>();

        httpClilent.BaseAddress = new(appConfig.CurrentValue.Giphy.BaseAddress);
    })
    .AddStandardResilienceHandler()
    .Services
    .AddHttpClient<IKlipyClient, KlipyClient>((services, httpClient) =>
    {
        var appConfig = services.GetRequiredService<IOptionsMonitor<AppConfig>>();

        httpClient.BaseAddress = new(appConfig.CurrentValue.Klipy.BaseAddress);
    })
    .AddStandardResilienceHandler();

var host = builder.Build();

var appConfig = host.Services.GetRequiredService<IOptions<AppConfig>>();
var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var gifPostingBehaviorSeeder = host.Services.GetRequiredService<IGifPostingBehaviorSeeder>();
var giphyDataStage = host.Services.GetRequiredService<IGiphyDataStage>();
var giphyRandomCache = host.Services.GetRequiredService<IGiphyRandomCache>();
var giphyTrendingCache = host.Services.GetRequiredService<IGiphyTrendingCache>();
var interactionService = host.Services.GetRequiredService<InteractionService>();
var intervalSeeder = host.Services.GetRequiredService<IIntervalSeeder>();
var klipyDataStage = host.Services.GetRequiredService<IKlipyDataStage>();
var klipyRandomCache = host.Services.GetRequiredService<IKlipyRandomCache>();
var klipyTrendingCache = host.Services.GetRequiredService<IKlipyTrendingCache>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

discordSocketClient.ButtonExecuted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.InteractionCreated += discordSocketClientHandler.OnInteractionCreatedAsync;
discordSocketClient.JoinedGuild += discordSocketClientHandler.OnJoinedGuildAsync;
discordSocketClient.LeftGuild += discordSocketClientHandler.OnLeftGuildAsync;
discordSocketClient.Log += discordSocketClientHandler.OnLogAsync;
discordSocketClient.ModalSubmitted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.Ready += discordSocketClientHandler.OnReadyAsync;
discordSocketClient.SelectMenuExecuted += discordSocketClientHandler.OnSocketInteractionAsync;

interactionService.Log += discordSocketClientHandler.OnLogAsync;

var configuration = builder.Configuration as IConfiguration;

ChangeToken.OnChange
(
    configuration.GetReloadToken,
    () =>
    {
        logger.LogInformation("Configuration has changed");

        if (builder.Environment.IsDevelopment())
        {
            var debugView = Environment.NewLine + builder.Configuration.GetDebugView().TrimEnd();

            logger.LogDebugView(debugView);
        }
    }
);

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
    await klipyTrendingCache.RefreshTrendingGifsAsync();

    await giphyRandomCache.RefreshRandomGifsAsync();
    await klipyRandomCache.RefreshRandomGifsAsync();

    await giphyDataStage.RefreshAsync();
    await klipyDataStage.RefreshAsync();

    await discordSocketClient.LoginAsync(TokenType.Bot, appConfig.Value.Discord.Token);
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