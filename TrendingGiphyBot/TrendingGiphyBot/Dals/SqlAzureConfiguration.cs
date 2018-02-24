using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace TrendingGiphyBot.Dals
{
    public class SqlAzureConfiguration : DbConfiguration
    {
        public SqlAzureConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}