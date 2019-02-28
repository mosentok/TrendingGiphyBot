namespace TrendingGiphyBotCore.Wrappers
{
    public interface IConfigurationWrapper
    {
        string this[string key] { get; }
        T GetValue<T>(string key);
        T Get<T>(string sectionName) where T : class;
    }
}