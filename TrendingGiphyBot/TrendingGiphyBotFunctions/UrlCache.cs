using System;
using System.ComponentModel.DataAnnotations;

namespace TrendingGiphyBotFunctions
{
    public class UrlCache
    {
        [Key]
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
    }
}
