using System;
using System.Runtime.Serialization;
using Discord;

namespace TrendingGiphyBotCore.Exceptions
{
    [Serializable]
    public class UnexpectedLogSeverityException : Exception
    {
        public UnexpectedLogSeverityException() { }
        public UnexpectedLogSeverityException(LogSeverity logSeverity) : this($"{logSeverity} is an unexpected {nameof(LogSeverity)}.") { }
        public UnexpectedLogSeverityException(string message) : base(message) { }
        public UnexpectedLogSeverityException(string message, Exception innerException) : base(message, innerException) { }
        protected UnexpectedLogSeverityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
