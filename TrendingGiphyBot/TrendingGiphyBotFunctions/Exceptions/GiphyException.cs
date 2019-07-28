using System;
using System.Runtime.Serialization;

namespace TrendingGiphyBotFunctions.Exceptions
{
    public class GiphyException : Exception
    {
        public string Response { get; set; }
        public GiphyException() { }
        public GiphyException(string message) : base(message) { }
        public GiphyException(string message, Exception innerException) : base(message, innerException) { }
        protected GiphyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public GiphyException(string message, Exception innerException, string response) : this(message, innerException)
        {
            Response = response;
        }
    }
}
