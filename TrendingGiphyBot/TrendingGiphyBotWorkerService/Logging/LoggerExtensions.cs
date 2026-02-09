using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Discord;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Logging;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "<{Method}> Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception, [CallerMemberName] string method = "");

	[LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save GifPost {GifPost} to the database.")]
    public static partial void LogGiphyPostingException(this ILogger logger, Exception exception, [LogProperties] GiphyPost gifPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save KlipyPost {KlipyPost} to the database.")]
    public static partial void LogKlipyPostingException(this ILogger logger, Exception exception, [LogProperties] KlipyPost klipyPost, [CallerMemberName] string method = "");

	[LoggerMessage(Message = "<{Method}> Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GiphyPost {GiphyPost}.")]
    public static partial void LogErrorPostingGiphy(this ILogger logger, Exception exception, string giphyDataId, ulong channelId, [LogProperties] GiphyPost giphyPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting KlipyDataId {KlipyDataId} to ChannelId {ChannelId}. Removing KlipyPost {KlipyPost}.")]
    public static partial void LogErrorPostingKlipy(this ILogger logger, Exception exception, ulong klipyDataId, ulong channelId, [LogProperties] KlipyPost klipyPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Giphy cache contains {SearchGiphyCacheCount} items.")]
    public static partial void LogGiphySearchCacheCount(this ILogger logger, int searchGiphyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search klipy cache contains {SearchKlipyCacheCount} klipy items.")]
    public static partial void LogKlipySearchCacheCount(this ILogger logger, int searchKlipyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache is refreshing.")]
    public static partial void LogGiphyRandomCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random klipy cache is refreshing.")]
    public static partial void LogKlipyRandomCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache has refreshed.")]
    public static partial void LogGiphyRandomCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random klipy cache has refreshed.")]
    public static partial void LogKlipyRandomCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache is refreshing.")]
    public static partial void LogGiphyTrendingCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending klipy cache is refreshing.")]
    public static partial void LogKlipyTrendingCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache has refreshed.")]
    public static partial void LogGiphyTrendingCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending klipy cache has refreshed.")]
    public static partial void LogKlipyTrendingCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache contains {TrendingGiphyCacheCount} items.")]
    public static partial void LogGiphyTrendingCacheCount(this ILogger logger, int trendingGiphyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending klipy cache contains {TrendingKlipyCacheCount} items.")]
    public static partial void LogKlipyTrendingCacheCount(this ILogger logger, int trendingKlipyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache contains {RandomGiphyCacheCount} items.")]
    public static partial void LogGiphyRandomCacheCount(this ILogger logger, int randomGiphyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random klipy cache contains {RandomKlipyCacheCount} items.")]
    public static partial void LogKlipyRandomCacheCount(this ILogger logger, int randomKlipyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage is refreshing.")]
    public static partial void LogGifStageIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage has refreshed.")]
    public static partial void LogGifStageHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The Giphy stage contains {GiphyStageCount} items.")]
    public static partial void LogGiphyStageCount(this ILogger logger, int giphyStageCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The klipy stage contains {KlipyStageCount} items.")]
    public static partial void LogKlipyStageCount(this ILogger logger, int klipyStageCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posting gifs.")]
    public static partial void LogPostingGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posted gifs.")]
    public static partial void LogPostedGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding gif posting behaviors.")]
    public static partial void LogSeedingGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded gif posting behaviors.")]
    public static partial void LogSeededGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding intervals.")]
    public static partial void LogSeedingIntervals(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded intervals.")]
    public static partial void LogSeededIntervals(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Valid minutes {ValidMinutes}.")]
    public static partial void LogValidMinutes(this ILogger logger, int[] validMinutes, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Valid hours {ValidHours}.")]
    public static partial void LogValidHours(this ILogger logger, int[] validHours, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Channel IDs in posting hours {ChannelIdsInPostingHours}.")]
    public static partial void LogChannelIdsInPostingHours(this ILogger logger, List<ulong> channelIdsInPostingHours, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Debug, Message = "<{Method}> Channel {ChannelId}: InPostingHours={InPostingHours}.")]
    public static partial void LogChannelInPostingHours(this ILogger logger, ulong channelId, bool inPostingHours, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Staged channel Giphy posts {StagedChannelGiphyPosts}.")]
    public static partial void LogStagedChannelGiphyPosts(this ILogger logger, IImmutableDictionary<ulong, GiphyData> stagedChannelGiphyPosts, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Staged channel klipy posts {StagedChannelKlipyPosts}.")]
    public static partial void LogStagedChannelKlipyPosts(this ILogger logger, IImmutableDictionary<ulong, KlipyData> stagedChannelKlipyPosts, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initializing.")]
    public static partial void LogInitializing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initialized.")]
    public static partial void LogInitialized(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Configuration:{DebugView}.")]
    public static partial void LogDebugView(this ILogger logger, string debugView, [CallerMemberName] string method = "");
}
