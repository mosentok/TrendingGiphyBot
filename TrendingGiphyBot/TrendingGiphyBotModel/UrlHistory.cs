using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrendingGiphyBotModel
{
    public class UrlHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(20,0)")]
        public decimal ChannelId { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string GifId { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
        public virtual JobConfig JobConfig { get; set; }
    }
}
