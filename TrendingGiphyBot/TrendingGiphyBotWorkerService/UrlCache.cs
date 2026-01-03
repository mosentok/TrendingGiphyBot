using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendingGiphyBotWorkerService;

public class UrlCache
{
	[Key]
	[Column(TypeName = "varchar(20)")]
	public string Id { get; set; }
	[Column(TypeName = "varchar(255)")]
	public string Url { get; set; }
	public DateTime Stamp { get; set; }
}
