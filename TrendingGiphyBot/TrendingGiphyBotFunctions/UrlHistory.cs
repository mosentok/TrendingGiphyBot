using System;
using System.ComponentModel.DataAnnotations;

namespace TrendingGiphyBotFunctions
{
    public class UrlHistory
    {
        public decimal ChannelId { get; set; }
        public string Url { get; set; }
        public DateTime Stamp { get; set; }
    }
}
