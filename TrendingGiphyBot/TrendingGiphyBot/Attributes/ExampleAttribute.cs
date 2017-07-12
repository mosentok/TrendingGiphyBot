using System;

namespace TrendingGiphyBot.Attributes
{
    public class ExampleAttribute : Attribute
    {
        public string Name => "Example";
        public string Text { get; set; }
        public ExampleAttribute() { }
        public ExampleAttribute(string text)
        {
            Text = text;
        }
    }
}
