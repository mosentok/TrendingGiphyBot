using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService.Configuration;

public static class ConfigurationExtensions
{
	public static string GetRequiredConfiguration(this IConfiguration configuration, string key) => configuration[key] ?? throw new MissingConfigurationException();
	public static T GetRequiredConfiguration<T>(this IConfiguration configuration, string key) => configuration.GetSection(key).Get<T>() ?? throw new MissingConfigurationException();
	public static T? GetOptionalConfiguration<T>(this IConfiguration configuration, string key) => configuration.GetSection(key).Get<T>();
}
