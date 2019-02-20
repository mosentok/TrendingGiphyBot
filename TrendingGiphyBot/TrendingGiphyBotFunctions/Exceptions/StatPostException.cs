using System;
using System.Runtime.Serialization;

namespace TrendingGiphyBotFunctions.Exceptions
{
    public class StatPostException : Exception
    {
        public StatPostException()
        {
        }

        public StatPostException(string message) : base(message)
        {
        }

        public StatPostException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StatPostException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
