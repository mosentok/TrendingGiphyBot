using System.Runtime.CompilerServices;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save GifPost {GifPost} to the database.")]
    public static partial void LogGiphyPostingException(this ILogger logger, Exception exception, [LogProperties] GiphyPost gifPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save KlipyPost {KlipyPost} to the database.")]
    public static partial void LogKlipyPostingException(this ILogger logger, Exception exception, [LogProperties] KlipyPost klipyPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GiphyPost {GiphyPost}.")]
    public static partial void LogErrorPostingGiphy(this ILogger logger, Exception exception, string giphyDataId, ulong channelId, [LogProperties] GiphyPost giphyPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting KlipyDataId {KlipyDataId} to ChannelId {ChannelId}. Removing KlipyPost {KlipyPost}.")]
    public static partial void LogErrorPostingKlipy(this ILogger logger, Exception exception, ulong klipyDataId, ulong channelId, [LogProperties] KlipyPost klipyPost, [CallerMemberName] string method = "");
}
