using System;
using System.Runtime.Serialization;
using Discord;

namespace TrendingGiphyBot.Exceptions
{
    [Serializable]
    public class InvalidLogSeverityException : Exception
    {
        public InvalidLogSeverityException() { }
        public InvalidLogSeverityException(LogSeverity logSeverity) : this($"{logSeverity} is an invalid {nameof(LogSeverity)}.") { }
        public InvalidLogSeverityException(string message) : base(message) { }
        public InvalidLogSeverityException(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidLogSeverityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
