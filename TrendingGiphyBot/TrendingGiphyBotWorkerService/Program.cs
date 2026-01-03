using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService;

var builder = Host.CreateApplicationBuilder(args);

var discordSocketClient = new DiscordSocketClient();

var discordToken = builder.Configuration.GetRequiredConfiguration("DiscordToken");
var playingGame = builder.Configuration.GetRequiredConfiguration("PlayingGame");

var bot = new Bot(discordSocketClient, discordToken, playingGame);

builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json")
	.AddEnvironmentVariables();

builder.Services
	.AddHostedService<Worker>()
	.AddLogging(builder => builder.AddConsole())
	.AddDbContext<ITrendingGiphyBotContext, TrendingGiphyBotContext>(builder =>
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		var databasePath = Path.Combine(currentDirectory, "app.db");

		builder.UseSqlite($"Data Source={databasePath}");
	})
	.AddSingleton(typeof(ILogger<>), typeof(Logger<>))
	.AddSingleton<DiscordSocketClient>()
	.AddSingleton(bot)
;


var host = builder.Build();
var listenToOnlyTheseChannels = builder.Configuration.GetRequiredSectionAs<List<ulong>>("ListenToOnlyTheseChannels");

await host.RunAsync();
