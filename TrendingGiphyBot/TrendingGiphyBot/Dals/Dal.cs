namespace TrendingGiphyBot.Dals
{
    public abstract class Dal
    {
        protected string ConnectionString { get; }
        protected Dal(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
