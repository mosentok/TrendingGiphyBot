using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendingGiphyBotModel
{
    public class GifFilter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(20,0)")]
        public decimal ChannelId { get; set; }
        [Column(TypeName = "varchar(32)")]
        public string Filter { get; set; }
        public virtual JobConfig JobConfig { get; set; }
    }
}
