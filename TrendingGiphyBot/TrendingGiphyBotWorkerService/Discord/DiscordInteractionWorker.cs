using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordInteractionWorker(
	ILoggerWrapper<DiscordInteractionWorker> _loggerWrapper,
	IDiscordSocketClientHandler _discordSocketClientHandler,
	IDiscordSocketClientWrapper _discordSocketClient,
	InteractionService _interactionService,
	DiscordWorkerConfig _discordWorkerConfig
) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			_discordSocketClient.ButtonExecuted += _discordSocketClientHandler.OnComponentExecuted;
			_discordSocketClient.InteractionCreated += _discordSocketClientHandler.OnInteractionCreated;
			_discordSocketClient.JoinedGuild += _discordSocketClientHandler.OnJoinedGuild;
			_discordSocketClient.LeftGuild += _discordSocketClientHandler.OnLeftGuild;
			_discordSocketClient.Log += _discordSocketClientHandler.OnLog;
			_discordSocketClient.ModalSubmitted += _discordSocketClientHandler.OnModalSubmitted;
			_discordSocketClient.Ready += _discordSocketClientHandler.OnReady;
			_discordSocketClient.SelectMenuExecuted += _discordSocketClientHandler.OnComponentExecuted;

			_interactionService.Log += _discordSocketClientHandler.OnLog;

			await _discordSocketClient.LoginAsync(TokenType.Bot, _discordWorkerConfig.DiscordToken);
			await _discordSocketClient.StartAsync();

			await Task.Delay(Timeout.Infinite, stoppingToken);
		}
		catch (Exception exception)
		{
			_loggerWrapper.LogTopLevelException(exception);
		}
		finally
		{
			await _discordSocketClient.LogoutAsync();
		}
	}
}
public class DiscordPostingWorker(ILoggerWrapper<DiscordPostingWorker> _loggerWrapper, ITrendingGiphyBotDbContext _trendingGiphyBotDbContext, IGifPostStage _gifPostStage, IDiscordSocketClientWrapper _discordSocketClientWrapper) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		while (!stoppingToken.IsCancellationRequested)
		{
			await timer.WaitForNextTickAsync(stoppingToken);

			var activeChannelIds = await _trendingGiphyBotDbContext.ChannelSettings.Where(s => s.HowOften != null).Select(s => s.ChannelId).ToListAsync();

			// TODO parallelize this loop?
			foreach (var channelId in activeChannelIds)
            {
                try
                {
                    var wasFound = _gifPostStage.TryGetGiphyData(channelId, out var giphyData);

                    if (!wasFound || giphyData is null)
                        throw new ThisShouldBeImpossibleException();

                    var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                    if (channel is not IMessageChannel messageChannel)
                        throw new ThisShouldBeImpossibleException();

                    await messageChannel.SendMessageAsync($"*Trending!* {giphyData.Url}");

                    _gifPostStage.Evict(channelId);
                }
                catch (Exception ex)
                {
					_loggerWrapper.LogGifPostingException(ex);
				}
            }
		}
	}
}
public class GifStagingWorker(IGifPostStage _gifPostStage) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

		while (!stoppingToken.IsCancellationRequested)
		{
			await timer.WaitForNextTickAsync(stoppingToken);

			_gifPostStage.Refresh();
		}
	}
}

public interface IGifPostStage
{
    bool TryGetGiphyData(ulong channelId, out GiphyData? giphyData);
    void Refresh();
    void Evict(ulong channelId);
}

public class GifPostStage(ITrendingGiphyBotDbContext _trendingGiphyBotDbContext, IGifCache _gifCache) : IGifPostStage
{
    readonly Dictionary<ulong, GiphyData> _channelGifPostStage = [];

    public void Evict(ulong channelId) => _channelGifPostStage.Remove(channelId);

    public void Refresh()
    {
        var activeChannels = _trendingGiphyBotDbContext.ChannelSettings.Where(s => s.HowOften != null);

        foreach (var channel in activeChannels)
        {
            if (_channelGifPostStage.ContainsKey(channel.ChannelId))
                continue;

            var seenGifIds = channel.GifPosts is not null
                ? channel.GifPosts.Select(s => s.GiphyDataId).ToList()
                : [];

            var firstUnseen = _gifCache.GetFirstUnseenGif(seenGifIds);

            if (firstUnseen is null)
            {
                // TODO this should be an enum or something
                if (channel.GifPostingBehavior == "POST WITH RANDOM GIF TODO")
                {
                    // TODO get and stage random gif based on GifKeyword
                }

                continue;
            }

            _channelGifPostStage[channel.ChannelId] = firstUnseen;
        }
    }

	public bool TryGetGiphyData(ulong channelId, out GiphyData? giphyData) => _channelGifPostStage.TryGetValue(channelId, out giphyData);
}