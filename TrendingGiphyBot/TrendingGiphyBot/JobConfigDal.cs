using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot
{
    class JobConfigDal
    {
        readonly string _ConnectionString;
        public JobConfigDal(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        internal Task<List<JobConfig>> GetAll()
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                    return dataContext.JobConfigs.ToList();
            });
        }
        internal Task<JobConfig> Get(ulong id)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                    return dataContext.JobConfigs.SingleOrDefault(s => s.ChannelId == id);
            });
        }
        internal Task<bool> Any(ulong id)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                    return dataContext.JobConfigs.Any(s => s.ChannelId == id);
            });
        }
        internal Task Insert(JobConfig config)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                {
                    dataContext.JobConfigs.InsertOnSubmit(config);
                    dataContext.SubmitChanges();
                }
            });
        }
        internal Task Update(JobConfig config)
        {
            return Task.Run(() =>
            {
                using (var dataContext = new TrendingGiphyBotDataContext(_ConnectionString))
                {
                    var match = dataContext.JobConfigs.Single(s => s.ChannelId == config.ChannelId);
                    match.Interval = config.Interval;
                    match.Time = config.Time.ToString();
                    dataContext.SubmitChanges();
                }
            });
        }
    }
}
