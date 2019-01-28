using System;
using System.ComponentModel.DataAnnotations;

namespace TrendingGiphyBotModel
{
    public class UrlCache
    {
        [Key]
        public string Id { get; set; }
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
    }
}
