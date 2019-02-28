using Microsoft.Extensions.Configuration;

namespace TrendingGiphyBotCore.Wrappers
{
    public class ConfigurationWrapper : IConfigurationWrapper
    {
        readonly IConfiguration _Config;
        public ConfigurationWrapper(IConfiguration config)
        {
            _Config = config;
        }
        public string this[string key] => _Config[key];
        public T GetValue<T>(string key) => _Config.GetValue<T>(key);
        public T Get<T>(string sectionName) where T : class => _Config.GetSection(sectionName).Get<T>();
    }
}
