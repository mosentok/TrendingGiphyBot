using System;
using System.Runtime.Serialization;

namespace TrendingGiphyBot
{
    [Serializable]
    public class InvalidTimeException : Exception
    {
        public InvalidTimeException() { }
        public InvalidTimeException(Time time) : this($"{time} is an invalid {nameof(Time)}.") { }
        public InvalidTimeException(string message) : base(message) { }
        public InvalidTimeException(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidTimeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
