using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService.Configuration;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public string GetRequiredConfiguration(string key) => configuration[key] ?? throw new MissingConfigurationException();
        public T GetRequiredConfiguration<T>(string key) => configuration.GetOptionalConfiguration<T>(key) ?? throw new MissingConfigurationException();
        public T? GetOptionalConfiguration<T>(string key) => configuration.GetSection(key).Get<T>();
    }
}
