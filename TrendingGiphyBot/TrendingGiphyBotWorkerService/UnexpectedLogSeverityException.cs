using Discord;

namespace TrendingGiphyBotWorkerService;

public class UnexpectedLogSeverityException : Exception
{
	public UnexpectedLogSeverityException() { }
	public UnexpectedLogSeverityException(LogSeverity logSeverity) : this($"{logSeverity} is an unexpected {nameof(LogSeverity)}.") { }
	public UnexpectedLogSeverityException(string message) : base(message) { }
	public UnexpectedLogSeverityException(string message, Exception innerException) : base(message, innerException) { }
}
