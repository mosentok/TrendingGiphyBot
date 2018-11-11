namespace TrendingGiphyBot.Dals
{
    public class EntitiesFactory
    {
        readonly string _ConnectionString;
        public EntitiesFactory(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        public TrendingGiphyBotEntities GetNewTrendingGiphyBotEntities()
        {
            return new TrendingGiphyBotEntities(_ConnectionString);
        }
    }
}
