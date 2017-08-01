using System;
using System.Runtime.Serialization;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Exceptions
{
    [Serializable]
    public class UnexpectedTimeException : Exception
    {
        public UnexpectedTimeException() { }
        public UnexpectedTimeException(Time time) : this($"{time} is an unexpected {nameof(Time)}.") { }
        public UnexpectedTimeException(string message) : base(message) { }
        public UnexpectedTimeException(string message, Exception innerException) : base(message, innerException) { }
        protected UnexpectedTimeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
