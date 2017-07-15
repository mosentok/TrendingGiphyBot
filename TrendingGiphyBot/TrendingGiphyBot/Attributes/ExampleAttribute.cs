using System;
using System.Linq;

namespace TrendingGiphyBot.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        public static string Name => "Example";
        public string[] Texts { get; }
        public ExampleAttribute(params string[] texts)
        {
            if (texts.Any())
                if (texts.All(s => !string.IsNullOrEmpty(s)))
                    Texts = texts;
                else
                    throw new InvalidOperationException($"{nameof(texts)} cannot contain null or the empty string.");
            else
                throw new InvalidOperationException($"Need at least one {texts}.");
        }
    }
}
