using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendingGiphyBotWorkerService;

public class JobConfig
{
	[Key]
	[Column(TypeName = "decimal(20,0)")]
	public decimal ChannelId { get; set; }
	public short? Interval { get; set; }
	[Column(TypeName = "varchar(7)")]
	public string Time { get; set; }
	[Column(TypeName = "varchar(32)")]
	public string RandomSearchString { get; set; }
	public short? MinQuietHour { get; set; }
	public short? MaxQuietHour { get; set; }
	public int? IntervalMinutes { get; set; }
	[Column(TypeName = "varchar(4)")]
	public string Prefix { get; set; }
	public virtual ICollection<UrlHistory> UrlHistories { get; set; }
}
