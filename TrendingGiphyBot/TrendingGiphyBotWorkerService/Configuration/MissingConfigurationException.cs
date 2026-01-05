namespace TrendingGiphyBotWorkerService.Configuration;

public class MissingConfigurationException : Exception
{
	public MissingConfigurationException() { }
	public MissingConfigurationException(string? message) : base(message) { }
	public MissingConfigurationException(string? message, Exception? innerException) : base(message, innerException) { }
}
