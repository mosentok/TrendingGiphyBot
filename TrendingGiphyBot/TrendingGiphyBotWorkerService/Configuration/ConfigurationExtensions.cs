using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace TrendingGiphyBotWorkerService.Configuration;

[SuppressMessage("", "S2325", Justification = "SonarQube hasn't been updated to handle the extensions keyword. SonarQube thinks these methods don't access instance data.")]
public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public string GetRequiredConfiguration(string key) => configuration[key] ?? throw new MissingConfigurationException();
        public T GetRequiredConfiguration<T>(string key) => configuration.GetOptionalConfiguration<T>(key) ?? throw new MissingConfigurationException();
        public T? GetOptionalConfiguration<T>(string key) => configuration.GetSection(key).Get<T>();
    }
}
