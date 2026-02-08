using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Discord;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Logging;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "<{Method}> Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception, [CallerMemberName] string method = "");

	[LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save GifPost {GifPost} to the database.")]
	public static partial void LogGifPostingException(this ILogger logger, Exception exception, [LogProperties] GiphyPost gifPost, [CallerMemberName] string method = "");

	[LoggerMessage(Message = "<{Method}> Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GifPost {GifPost}.")]
    public static partial void LogErrorPostingGif(this ILogger logger, Exception exception, string giphyDataId, ulong channelId, [LogProperties] GiphyPost gifPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search gif cache contains {SearchGifCacheCount} gifs.")]
    public static partial void LogGifSearchCacheCount(this ILogger logger, int searchGifCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending if cache is refreshing.")]
    public static partial void LogGifTrendingCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending gif cache has refreshed.")]
    public static partial void LogGifTrendingCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending gif cache contains {TrendingGifCacheCount} gifs.")]
    public static partial void LogGifTrendingCacheCount(this ILogger logger, int trendingGifCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage is refreshing.")]
    public static partial void LogGifStageIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage has refreshed.")]
    public static partial void LogGifStageHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage contains {GifStageCount} gifs.")]
    public static partial void LogGifStageCount(this ILogger logger, int gifStageCount, [CallerMemberName] string method = "");

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

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Staged channel gif posts {StagedChannelGifPosts}.")]
    public static partial void LogStagedChannelGifPosts(this ILogger logger, IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initializing.")]
    public static partial void LogInitializing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initialized.")]
    public static partial void LogInitialized(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Configuration:{DebugView}.")]
    public static partial void LogDebugView(this ILogger logger, string debugView, [CallerMemberName] string method = "");
}
