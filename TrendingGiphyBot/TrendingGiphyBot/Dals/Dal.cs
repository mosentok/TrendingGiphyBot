namespace TrendingGiphyBot.Dals
{
    abstract class Dal
    {
        protected string ConnectionString { get; }
        protected Dal(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
