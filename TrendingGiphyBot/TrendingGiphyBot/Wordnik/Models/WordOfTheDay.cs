using System;
using System.Collections.Generic;

namespace TrendingGiphyBot.Wordnik.Models
{
    class WordOfTheDay
    {
        public ulong Id { get; set; }
        public string Word { get; set; }
        public string Note { get; set; }
        public DateTime PublishDate { get; set; }
        public ContentProvider ContentProvider { get; set; }
        public List<Example> Examples { get; set; }
        public List<Defintion> Definitions { get; set; }
    }
}
