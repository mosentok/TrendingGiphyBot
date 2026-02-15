using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posting gifs.")]
    public static partial void LogPostingGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posted gifs.")]
    public static partial void LogPostedGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Channel IDs in posting hours {ChannelIdsInPostingHours}.")]
    public static partial void LogChannelIdsInPostingHours(this ILogger logger, List<ulong> channelIdsInPostingHours, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Staged channel Giphy posts {StagedChannelGiphyPosts}.")]
    public static partial void LogStagedChannelGiphyPosts(this ILogger logger, IImmutableDictionary<ulong, GiphyData> stagedChannelGiphyPosts, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Staged channel Klipy posts {StagedChannelKlipyPosts}.")]
    public static partial void LogStagedChannelKlipyPosts(this ILogger logger, IImmutableDictionary<ulong, KlipyData> stagedChannelKlipyPosts, [CallerMemberName] string method = "");
}
