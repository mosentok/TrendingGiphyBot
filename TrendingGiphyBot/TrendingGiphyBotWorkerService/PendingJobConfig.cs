namespace TrendingGiphyBotWorkerService;

public class PendingJobConfig
{
	public decimal ChannelId { get; set; }
	public string RandomSearchString { get; set; }
	public List<PendingHistory> Histories { get; set; }
}
