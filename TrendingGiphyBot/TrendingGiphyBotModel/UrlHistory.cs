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
        public decimal ChannelId { get; set; }
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
        public virtual JobConfig JobConfig { get; set; }
    }
}
