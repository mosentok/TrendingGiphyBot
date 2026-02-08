using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using TrendingGiphyBotWorkerService;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Discord.Delaying;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Klipy.Staging;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Logging;
using TrendingGiphyBotWorkerService.Utc;

[assembly: SuppressMessage("Roslynator", "RCS1001:Add braces (when expression spans over multiple lines)", Justification = "Less is more.")]

var builder = Host.CreateApplicationBuilder(args);

var currentDirectory = Directory.GetCurrentDirectory();
var databasePath = Path.Combine(currentDirectory, "app.db");
var connectionString = $"Data Source={databasePath}";

builder.Configuration
    .SetBasePath(currentDirectory)
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables("Tgb2__");

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

var logPath = Path.Combine(currentDirectory, "logs", "log.log");

const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u4}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

builder.Services
    .Configure<AppConfig>(builder.Configuration)
    .AddHostedService<DiscordPostingWorker>()
    .AddHostedService<GifStagingWorker>()
    .AddHostedService<GiphyCacheWorker>()
    .AddHostedService<KlipyCacheWorker>()
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
    .AddSingleton(discordSocketClient)
    .AddSingleton(services =>
    {
        var appConfig = services.GetRequiredService<IOptions<AppConfig>>();

        return new InteractionService(discordSocketClient, new() { UseCompiledLambda = true, LogLevel = appConfig.Value.Discord.LogSeverity, DefaultRunMode = RunMode.Async });
    })
    .AddSingleton(TimeProvider.System)
    .AddTrendingGiphyBotWorkerService()
    .AddHttpClient<IGiphyClient, GiphyClient>((services, httpClilent) =>
    {
        var appConfig = services.GetRequiredService<IOptions<AppConfig>>();

        httpClilent.BaseAddress = new(appConfig.Value.Giphy.BaseAddress);
    })
    .AddStandardResilienceHandler()
    .Services
    .AddHttpClient<IKlipyClient, KlipyClient>((services, httpClient) =>
    {
        var appConfig = services.GetRequiredService<IOptions<AppConfig>>();

        httpClient.BaseAddress = new(appConfig.Value.Klipy.BaseAddress);
    })
    .AddStandardResilienceHandler();

var host = builder.Build();

var discordSocketClientHandler = host.Services.GetRequiredService<IDiscordSocketClientHandler>();
var gifPostingBehaviorSeeder = host.Services.GetRequiredService<IGifPostingBehaviorSeeder>();
var giphyDataStage = host.Services.GetRequiredService<IGiphyDataStage>();
var giphyTrendingCache = host.Services.GetRequiredService<IGiphyTrendingCache>();
var intervalSeeder = host.Services.GetRequiredService<IIntervalSeeder>();
var klipyDataStage = host.Services.GetRequiredService<IKlipyDataStage>();
var klipyTrendingCache = host.Services.GetRequiredService<IKlipyTrendingCache>();

discordSocketClient.ButtonExecuted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.InteractionCreated += discordSocketClientHandler.OnInteractionCreatedAsync;
discordSocketClient.JoinedGuild += discordSocketClientHandler.OnJoinedGuildAsync;
discordSocketClient.LeftGuild += discordSocketClientHandler.OnLeftGuildAsync;
discordSocketClient.Log += discordSocketClientHandler.OnLogAsync;
discordSocketClient.ModalSubmitted += discordSocketClientHandler.OnSocketInteractionAsync;
discordSocketClient.Ready += discordSocketClientHandler.OnReadyAsync;
discordSocketClient.SelectMenuExecuted += discordSocketClientHandler.OnSocketInteractionAsync;

var appConfig = host.Services.GetRequiredService<IOptions<AppConfig>>();
var interactionService = host.Services.GetRequiredService<InteractionService>();

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
    await klipyTrendingCache.RefreshTrendingGifsAsync();
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